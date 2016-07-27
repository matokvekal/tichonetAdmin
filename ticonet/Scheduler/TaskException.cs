using FluentScheduler.Model;

using System;

namespace ticonet.Scheduler
{
    public static class TaskException
    {
        public static void TaskManager_UnobservedTaskException(TaskExceptionInformation i, UnhandledExceptionEventArgs a)
        {
            //Log.Fatal("An error happened with a scheduled task: " + e.ExceptionObject);
        }
    }
}