using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Business_Logic
{
    public class tblSettingLogic : baseLogic
    {
        public tblSetting Get() {
            tblSetting setting = DB.tblSettings.FirstOrDefault(x => x.Id==0 );
            if (setting == null) {
                setting = new tblSetting
                {
                    Id = 0,
                    PopulateLinesIsActive = null,
                    PopulateLinesLastRun = null,
                };
                DB.tblSettings.Add(setting);
                DB.SaveChanges();
            }
            return setting;
        }
        public bool PopulateLinesIsActive{
            get {
                var setting = Get();
                return setting.PopulateLinesIsActive.HasValue ? setting.PopulateLinesIsActive.Value : false;
            }
            set {
                var setting = Get();
                setting.PopulateLinesIsActive = value;
                DB.Entry(setting).State = EntityState.Modified;
                DB.SaveChanges();
            }
        }
        public DateTime? PopulateLinesLastRun {
            get {
                var setting = Get();
                return setting.PopulateLinesLastRun;
            }
            set {
                if (value == null) return;
                var setting = Get();
                setting.PopulateLinesLastRun = value;
                DB.Entry(setting).State = EntityState.Modified;
                DB.SaveChanges();
            }
        }
        public void UpdateConfig(IDictionary<string, object> settings) {
            foreach (var kv in settings) {
                var prop = GetType().GetProperty(kv.Key);
                if (prop != null) {
                    //TODO
                    //prop.DeclaringType;
                    bool res;
                    res = ((string[])kv.Value)[0] == "true";
                    prop.SetValue(this, res);
                }
            }
        }
        public IDictionary<string, object> GetConfig(IEnumerable<string> confNames) {
            var dict = new Dictionary<string, object>();
            foreach (var name in confNames) {
                var prop = GetType().GetProperty(name);
                if (prop != null)
                    dict.Add(name, prop.GetValue(this));
            }
            return dict;
        }
    }
}
