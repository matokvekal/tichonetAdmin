using System;
using System.Data.Entity;
using System.Linq;

namespace Business_Logic
{
    public class tblSettingLogic : baseLogic
    {

        public tblSetting Get()
        {
            tblSetting setting = DB.tblSettings.FirstOrDefault(x => x.Id==0 );
            if (setting == null)
                setting = new tblSetting
                {
                    Id = 0,
                    PopulateLinesIsActive = null,
                    PopulateLinesLastRun = null,
                };

            return setting;
        }

        public bool GetPopulateLinesIsActive()
        {
            var setting = Get();
            return setting.PopulateLinesIsActive.HasValue ? setting.PopulateLinesIsActive.Value : false;
        }

        public bool SetPopulateLinesIsActive(bool isActive)
        {
            var res = false;
            try
            {
                var setting = Get();
                setting.PopulateLinesIsActive = isActive;
                BusProjectEntities db = new BusProjectEntities();
                db.Entry(setting).State = EntityState.Modified;
                db.SaveChanges();
                res = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return res;
        }

        public DateTime? GetPopulateLinesLastRun()
        {
            var setting = Get();
            return setting.PopulateLinesLastRun;
        }

        public bool SetPopulateLinesLastRun(DateTime lastRun)
        {
            var res = false;
            try
            {
                var setting = Get();
                setting.PopulateLinesLastRun = lastRun;
                BusProjectEntities db = new BusProjectEntities();
                db.Entry(setting).State = EntityState.Modified;
                db.SaveChanges();
                res = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return res;
        }

    }
}
