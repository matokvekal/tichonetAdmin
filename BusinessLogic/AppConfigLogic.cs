using System;
using System.Collections.Generic;

namespace Business_Logic {

    //this is a mock
    public class AppConfigLogic : IDisposable {

        static bool mock_UseLinesPlanInAutoScheduling = true;

        public bool UseLinesPlanInAutoScheduling {
            get { return mock_UseLinesPlanInAutoScheduling; }
            set { mock_UseLinesPlanInAutoScheduling = value; }
        }

        public void Dispose() {
            //...
        }

        public void UpdateConfig (IDictionary<string,object> settings) {
            foreach (var kv in settings) {
                var prop = typeof(AppConfigLogic).GetProperty(kv.Key);
                //TODO
                bool res;
                res = ((string[])kv.Value)[0] == "true";
                prop.SetValue(this, res);
            }
        }

        public IDictionary<string,object> GetConfig (IEnumerable<string> confNames) {
            var dict = new Dictionary<string, object>();
            foreach (var name in confNames) {
                var prop = typeof(AppConfigLogic).GetProperty(name);
                if (prop != null)
                    dict.Add(name, prop.GetValue(this));
            }
            return dict;
        }
    }
}
