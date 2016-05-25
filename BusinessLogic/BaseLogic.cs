using System;

namespace Business_Logic
{
    public class baseLogic : IDisposable
    {
        private BusProjectEntities _db = new BusProjectEntities();

        protected BusProjectEntities DB
        {
            get
            {
                return _db;
            }
        }

        public void Dispose()
        {
            _db.Dispose();
        }


    }
  
}


