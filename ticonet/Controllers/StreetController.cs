using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Business_Logic;
using log4net;

namespace ticonet
{
    public class StreetController : Controller
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(StreetController));

        public bool checkstreet(string streetName)
        {
            using(tblStreetsLogic street=new tblStreetsLogic())
            {
                return street.IsExist(streetName);
            }

        }

        public string getStreetList(string id)
        {
            if (!string.IsNullOrEmpty(id) && id.Length > 2)
            {

                List<tblStreet> selected = null;
                if (HttpRuntime.Cache["streets"] == null)
                {
                    using (tblStreetsLogic streets = new tblStreetsLogic())
                    {
                        List<tblStreet> c = streets.GetStreets();
                        HttpRuntime.Cache.Insert("streets", c, null, Cache.NoAbsoluteExpiration, TimeSpan.FromHours(5), CacheItemPriority.High, null);
                        selected = c.Where(O => O.streetName.Contains(id.Trim())).ToList();
                    }
                }
                else
                {
                    List<tblStreet> c = HttpRuntime.Cache["streets"] as List<tblStreet>;

                    selected = c.Where(O => O.streetName.Contains(id.Trim())).ToList();
                }
                var jsonSerialiser = new JavaScriptSerializer();
                var list = jsonSerialiser.Serialize(selected);
                return list;
       
                //OLD VER BY DB
                //using (tblStreetsLogic streets = new tblStreetsLogic())
                //{
                //    List<tblStreet> c = streets.GetStreetsByprefix(id);


                //    var jsonSerialiser = new JavaScriptSerializer();
                //    var list = jsonSerialiser.Serialize(c);
                //    return list;
                //}

            }
            return null;

        }

    }
}





