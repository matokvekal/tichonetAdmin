using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.ServiceProcess;
using System.Windows.Forms;

namespace wsTiconet
{
    [RunInstaller(true)]
    public class CustomServiceInstaller : Installer
    {
        private ServiceProcessInstaller process;
        private ServiceInstaller service;

        private void RemoveIfExists(string serviceName)
        {
            if (ServiceController.GetServices().Any(s => s.ServiceName.ToLower() == serviceName.ToLower()))
            {
                Uninstall(null);
            }
        }

        public CustomServiceInstaller()
        {
            process = new ServiceProcessInstaller();
            process.Account = ServiceAccount.User;
            service = new ServiceInstaller();
            Installers.Add(process);
            Installers.Add(service);
        }


        protected override void OnBeforeInstall(IDictionary savedState)
        {
            AuthForm form = new AuthForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                process.Username = WindowsIdentity.GetCurrent().Name;
                string sname = "";
                string pwd = "";
                form.Get(out sname, out pwd);
                process.Password = pwd;
                service.ServiceName = sname;
                service.Description = Assembly.GetExecutingAssembly().GetName().Name;
                service.StartType = ServiceStartMode.Automatic;
                RemoveIfExists(sname);
                base.OnBeforeInstall(savedState);
            }
            else
            {
                throw new System.Exception("Operation canceled by user");
            }
        }

        public void Uninstall(string serviceName)
        {
            service.ServiceName = serviceName;
            base.Uninstall(null);
        }
    }
}
