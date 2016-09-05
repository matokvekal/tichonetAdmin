using System.Diagnostics;

namespace wsTiconet
{
    public class TiconetService : ITiconetService
    {

        private System.Timers.Timer ServiceTimer = new System.Timers.Timer();
        public TiconetService()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            ServiceTimer.Elapsed += WorkService;
        }

        public void OnStart(string[] args)
        {
            WriteToEventLog("Start", EventLogEntryType.Warning);
            ServiceTimer.Enabled = true;
            ServiceTimer.AutoReset = true;
            ServiceTimer.Interval = 5000; // Interval
            ServiceTimer.Start();
        }

        public void OnStop()
        {
            ServiceTimer.Stop();
            WriteToEventLog("Stop", EventLogEntryType.Warning);
        }

        public void WorkService(object sender, System.Timers.ElapsedEventArgs e)
        {
            ServiceTimer.Stop();

            //TODO Work Loop

            ServiceTimer.Start();
        }


        private readonly static string ServiceName = "Service Ticonet";
        private readonly static string LogName = "TiconetInfo";

        public static void WriteToEventLog(string Message, EventLogEntryType MessageType)
        {
            System.Diagnostics.EventLog GetValLog = new EventLog(LogName);
            GetValLog.Source = LogName;
            try
            {
                GetValLog.WriteEntry(Message, MessageType);
                //GetValLog.WriteEntry(Message, EventLogEntryType.Information);
            }
            catch { }
        }

    }
}
