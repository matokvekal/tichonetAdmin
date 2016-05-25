using System;
using System.Linq;

namespace Business_Logic.Helpers
{
    public static class SettingsHelper
    {
        public static  string GetSettingValue(string ns, string key)
        {
            string res = null;
            using (var context = new BusProjectEntities())
            {
                var entity =
                    context.tblSystems.FirstOrDefault(
                        z => z.strNamespace.ToLower() == ns.ToLower() && z.strKey.ToLower() == key.ToLower());
                if (entity != null)
                {
                    res = entity.strValue;
                }
            }
            return res;
        }

        public static  void SetSettingValue(string ns, string key, string value)
        {
            using (var context = new BusProjectEntities())
            {
                       var entity =
                    context.tblSystems.FirstOrDefault(
                        z => z.strNamespace.ToLower() == ns.ToLower() && z.strKey.ToLower() == key.ToLower());
                if (entity != null)
                {
                    entity.strValue = value;
                    entity.LastModify = DateTime.UtcNow;
                    if (AccountManager.LoginInfo != null) entity.ModifedBy = AccountManager.LoginInfo.UserId;
                }
                else
                {
                    entity = new tblSystem()
                    {
                        strNamespace = ns,
                        strKey = key,
                        strValue = value,
                        LastModify = DateTime.UtcNow 
                    };
                    if (AccountManager.LoginInfo != null) entity.ModifedBy = AccountManager.LoginInfo.UserId;
                    context.tblSystems.Add(entity);
                }
                context.SaveChanges();
            }
        }


    }
}