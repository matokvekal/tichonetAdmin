using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Business_Logic;


namespace Business_Logic
{


    public class dictSystemLogic : baseLogic
    {
        public static List<tblDictSystem> getAllTblDict()
        {
            try
            {
                BusProjectEntities db = new BusProjectEntities();
                return db.tblDictSystems.ToList();

            }
            catch
            {
                return null;
            }
        }

        public static List<tblDictSystem> getAllChangedTblDict(string culture)
        {
            try
            {
                BusProjectEntities db = new BusProjectEntities();
                List<tblDictSystem> v = (from o in db.tblDictSystems
                                         where ((o.cantBeChange != true ||
                                         o.cantBeChange == null) &&
                                          o.cultures == culture)
                                         select o).Distinct().ToList();
                //v = v.Where(o => o.cantBeChange != true);

                return v.ToList();

            }
            catch
            {
                return null;
            }
        }
        public static Dictionary<string, string> getDictionaryTerms()
        {

            BusProjectEntities db = new BusProjectEntities();
                string culture = "He_iL";
            //string culture = constants.Const_defaultCulture;
            Dictionary<string, string> terms = new Dictionary<string, string>();
            List<tblDictSystem> v = (from o in db.tblDictSystems
                                     where o.cultures == culture
                                     select o).Distinct().ToList();
            bool mod = false;
            if (System.Web.HttpRuntime.Cache["Dict-SYS"] != null)
                mod = Convert.ToBoolean(HttpRuntime.Cache["Dict-SYS"]);


            foreach (tblDictSystem term in v)//gilad 1-2015 try add id
            {
                try//gilad 1-2015 try add id
                {
                    if (term.CustomerTranslated == null)
                        terms.Add(term.term, term.SystemTranslated);
                    else if (mod)//alow the user to see the system term id
                        terms.Add(term.term, term.CustomerTranslated + "(DS-" + term.id + ")");
                    else
                        terms.Add(term.term, term.CustomerTranslated);
                }
                catch
                {
                    //todo log error
                }
            }

            return terms;

        }

        public static List<Object> getTermsFromTblDict()
        {
            try
            {
                BusProjectEntities db = new BusProjectEntities();
                //todo fix logic to take firts the customer translate than the system translate
                string culture = "He_iL";
                //Mamritz12Entities db = Mamritz12Entities.New();
                var v = (from o in db.tblDictSystems
                         where o.cultures == culture &&
                                              o.CustomerTranslated != null
                         select (new { o.term, o.CustomerTranslated })).ToList();
                return v.ToList<Object>();
                //return query.ToList<Object>();
                //  DB.tblDicts.Select(c => new { c.term, c.CustomerTranslated });

            }
            catch
            {
                return null;
            }
        }

        public static string returnTranslatedTerm(string term)
        {
            try
            {
                BusProjectEntities db = new BusProjectEntities();
                string culture = "He_iL";
                var v = (from o in db.tblDictSystems
                         where o.cultures == culture &&
                         o.term == term
                         select o).FirstOrDefault(); ;
                if (v != null)
                {
                    if (v.CustomerTranslated == null)
                    {

                        if (v.SystemTranslated != null)
                            return v.SystemTranslated;
                        else return "";
                    }
                    else return v.CustomerTranslated;
                }
                return null;

            }
            catch
            {
                return null;
            }
        }

        public static tblDictSystem getTblDict(int id)
        {
            try
            {
                BusProjectEntities db = new BusProjectEntities();
                return db.tblDictSystems.FirstOrDefault(c => c.id == id);

            }
            catch
            {
                return null;
            }
        }


        //Todo- get from tblsystemConterol the current culture 
        public static void addTblDict(tblDictSystem c)
        {
            try
            {
                BusProjectEntities db = new BusProjectEntities();
                db.tblDictSystems.Add(c);
                db.SaveChanges();
            }
            catch
            {
            }
        }
        public static void updateTblDictById(int dictId, string CustomerTranslated)
        {
            try
            {
                tblDictSystem c = getTblDict(dictId);
                c.CustomerTranslated = CustomerTranslated;
                c.DateCreatedUTC = DateTime.Now;
                updateTblDict(c);
            }
            catch
            {
            }
        }
        public static void updateTblDict(tblDictSystem c)
        {
            try
            {
                BusProjectEntities db = new BusProjectEntities();
                db.Entry<tblDictSystem>(c).State = EntityState.Modified; 
                db.SaveChanges();
            }
            catch
            {
            }
        }
        public static void deleteTblDict(int id)
        {
            try
            {
                BusProjectEntities db = new BusProjectEntities();
                db.Entry<tblDictSystem>(new tblDictSystem { id = id }).State = EntityState.Deleted;
                db.SaveChanges();
            }
            catch
            {
            }
        }
    }
}
