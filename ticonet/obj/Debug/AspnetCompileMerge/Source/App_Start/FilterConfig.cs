
using System.Web.Mvc;
using System.Data.Entity.Infrastructure;

namespace IdentitySample
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //all site error handle
            filters.Add(new HandleErrorAttribute());
            filters.Add(new AuthorizeAttribute());
  
            //all site DataBase error handle  for databse exception 
            HandleErrorAttribute dbException = new HandleErrorAttribute();
            dbException.ExceptionType = typeof(DbUpdateException);
            dbException.View = "DatabaseExeptionView";
            filters.Add(dbException);
        }
    }
}