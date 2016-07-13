using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;



namespace Business_Logic
{
    public class localizedSystemDisplayNameAttribute : DisplayNameAttribute
    {
        //  static Dictionary<string, string> dictunary = new Dictionary<string, string>();
        static Dictionary<string, string> dictunarySystem = HttpRuntime.Cache["dictSystem"] as Dictionary<string, string>;

        public static void cleanDictunary()
        {
            dictunarySystem = null;
            HttpRuntime.Cache.Remove("dictSystem");
        }
        public localizedSystemDisplayNameAttribute(string term)
            : base(GetMessageFromResource(term))
        { }

        public static string GetMessageFromResource(string Term)
        {
            return termHandle(Term);
        }

        public static string termHandle(string Term)
        {
            if (dictunarySystem == null)
            {
                if (HttpRuntime.Cache["dictSystem"] == null)
                {
                    dictunarySystem = dictSystemLogic.getDictionaryTerms();
                    handleCache();
                }
                else
                {//fil the dictunary from the cache
                    dictunarySystem = HttpRuntime.Cache["dictSystem"] as Dictionary<string, string>;
                }
            }
            string item;
            if (!dictunarySystem.TryGetValue(Term, out item))
            {
                string value = dictSystemLogic.returnTranslatedTerm(Term);
                if (value != null)
                {
                    dictunarySystem[Term] = value;
                }
                else
                {
                    value = Term;
                    dictunarySystem[Term] = Term;
                    tblDictSystem c = new tblDictSystem();//todo fix 
                    c.term = Term;
                    c.SystemTranslated = Term;
                    c.DateCreatedUTC = DateTime.Now;
                    //unncomment gilad 14-11-15 -1
                    //c.cultures = constants.Const_defaultCulture; //todo fix 
                    c.cultures = "He_iL";
                    dictSystemLogic.addTblDict(c);
                    // dictunary[Term] = Term;
                }
                handleCache();
                return Term;
            }
            else
                return dictunarySystem[Term];
        }

        private static void cacheRemuved(string key, object value, System.Web.Caching.CacheItemRemovedReason reason)
        {
            dictunarySystem.Clear();
            dictunarySystem = dictSystemLogic.getDictionaryTerms();
            handleCache();
        }

        public static void handleCache()
        {
            HttpRuntime.Cache.Remove("dictSystem");
            //HttpRuntime.Cache.Insert("dictSystem", dictunarySystem, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromHours(constants.Const_CacheTimeSpan), System.Web.Caching.CacheItemPriority.Normal, null);  //unncomment gilad 14-11-15 - 3
            HttpRuntime.Cache.Insert("dictSystem", dictunarySystem, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromHours(24), System.Web.Caching.CacheItemPriority.Normal, null);//

        }

    }




}

