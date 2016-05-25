namespace Business_Logic
{
    public class tblAlertsQueueLogic : baseLogic
    {

        public static void create(tblAlertsQueue c)
        {
            try
            {
                BusProjectEntities db = new BusProjectEntities();
                c.id = 9999;

                db.SaveChanges();

            }
            catch
            {

            }

        }
    }
}
