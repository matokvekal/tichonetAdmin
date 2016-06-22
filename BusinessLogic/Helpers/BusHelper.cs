using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.Helpers
{
    public class BusHelper
    {
        public const int DefaultLoadTime = 180;
        /// <summary>
        /// Time for load students to bus in secondsS
        /// </summary>
        public static int TimeForLoad
        {
            get
            {
                var st = SettingsHelper.GetSettingValue("Bus", "TimeForLoad");
                int res;
                if (int.TryParse(st, out res))
                {
                    return res;
                }
                else
                {
                    return DefaultLoadTime;
                }
            }
            set
            {
                SettingsHelper.SetSettingValue("Bus", "TimeForLoad", value.ToString());
            }
        }
    }
}
