using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using Business_Logic.Entities;

namespace Business_Logic
{
    public class tblBusCompanyLogic : baseLogic
    {

        public tblBusCompany GetBusCompanyById(int id)
        {
            try
            {
                BusProjectEntities db = new BusProjectEntities();
                return db.tblBusCompanies.FirstOrDefault(c => c.pk == id);
            }
            catch
            {
                return null;
            }
        }
        public List<tblBusCompany> GetBusCompanies()
        {
            try
            {
                BusProjectEntities db = new BusProjectEntities();
                List<tblBusCompany> c = db.tblBusCompanies.ToList();
                return c;
            }
            catch
            {
                return null;
            }

        }

    }
}
