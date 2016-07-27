using System;

namespace Business_Logic
{
    public class baseLogic : IDisposable
    {
        public baseLogic () {
            _db = new BusProjectEntities();
        }

        public baseLogic (BusProjectEntities openedContext) {
            _db = openedContext;
        }

        private BusProjectEntities _db;

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


