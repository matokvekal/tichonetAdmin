using FluentScheduler;

using System.Web.Mvc;


namespace ticonet.Scheduler
{
    public class NinjectTaskFactory : TaskFactory, ITaskFactory
    {
        public override ITask GetTaskInstance<T>()
        {
            return DependencyResolver.Current.GetService<T>();
        }
    }
}