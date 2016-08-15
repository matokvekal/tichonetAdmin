[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(ticonet.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(ticonet.App_Start.NinjectWebCommon), "Stop")]

namespace ticonet.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;
    using Scheduler.Tasks;
    using System.Data.Entity;
    using Business_Logic.MessagesContext;
    using Business_Logic.SqlContext;
    using System.Web.Configuration;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            //---------------------------------------------------------------------------------------------------------------------
            // Bind Tasks - Start
            //---------------------------------------------------------------------------------------------------------------------
            kernel.Bind<ITaskExample>().To<TaskExample>().InThreadScope();
            //---------------------------------------------------------------------------------------------------------------------
            // Bind Tasks - End
            //---------------------------------------------------------------------------------------------------------------------

            //---------------------------------------------------------------------------------------------------------------------
            // Bind Context - Start
            //---------------------------------------------------------------------------------------------------------------------
            kernel.Bind<IDbConnectionFactory>()
                .To<SqlConnectionFactory>()
                .WithConstructorArgument("connectionString",
            WebConfigurationManager.ConnectionStrings["DbConnectionString"].ConnectionString);
            kernel.Bind(typeof(ISqlLogic)).To(typeof(SqlLogic)).InRequestScope();
            //---------------------------------------------------------------------------------------------------------------------
            // Bind Context - End
            //---------------------------------------------------------------------------------------------------------------------

        }
    }
}
