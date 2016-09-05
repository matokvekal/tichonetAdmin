using Microsoft.Win32;
using Ninject;
using System;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using System.Threading;

//-----------------------------------------------------------------------------
//http://www.codeproject.com/Tips/855152/Windows-Self-installing-Named-Services
//-----------------------------------------------------------------------------
namespace wsTiconet
{
    class Program : ServiceBase
    {
        static void Main(string[] args)
        {
            bool debugMode = false;
            if (args.Length > 0)
            {
                for (int ii = 0; ii < args.Length; ii++)
                {
                    switch (args[ii].ToUpper())
                    {
                        case "/I":
                            InstallService();
                            return;
                        case "/U":
                            UninstallService();
                            return;
                        default:
                            break;
                    }
                }
            }

#if DEBUG
            Program service = new Program();
            service.OnStart(null);
            //Service Started
            bool loop = true;
            while(loop)
            {
                Thread.Sleep(5000);
            }
            service.OnStop();
#else
            System.ServiceProcess.ServiceBase.Run(new Program());
#endif
        }

        private ITiconetService _ticonetService;
        public Program()
        {
            var kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());

            _ticonetService = kernel.Get<ITiconetService>();
        }

        protected override void OnStart(string[] args)
        {
            _ticonetService.OnStart(args);
        }

        protected override void OnStop()
        {
            _ticonetService.OnStop();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        private static void InstallService()
        {
            try
            {
                ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });
            }
            catch { }
        }

        private static void UninstallService()
        {
            string binpath = Assembly.GetExecutingAssembly().Location;
            string dir = binpath.Remove(binpath.LastIndexOf("\\"));
            var toBeRemoved = ServiceController.GetServices().Where(s => GetImagePath(s.ServiceName) == binpath).Select(x => x.ServiceName);
            CustomServiceInstaller installer = new CustomServiceInstaller();
            installer.Context = new InstallContext();
            foreach (var sname in toBeRemoved)
            {
                try
                {
                    installer.Uninstall(sname);
                }
                catch { }
            }
        }

        static Regex pathrx = new Regex("(?<=\").+(?=\")");

        private static string GetImagePath(string servicename)
        {
            string registryPath = @"SYSTEM\CurrentControlSet\Services\" + servicename;
            RegistryKey keyHKLM = Registry.LocalMachine;
            RegistryKey key;
            key = keyHKLM.OpenSubKey(registryPath);
            string value = key.GetValue("ImagePath").ToString();
            key.Close();
            var result = pathrx.Match(value);
            if (result.Success)
                return result.Value;
            return value;
        }
    }
}