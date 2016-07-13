using System;
using System.Collections.Generic;
using Business_Logic;

namespace ticonet.Models
{
    public class BusCompanyModel
    {
        public BusCompanyModel(tblBusCompany data)
        {
            pk = data.pk;
            companyName = data.companyName;
            manager = data.manager;
            tel = data.tel;
            cell = data.cell;
            email = data.email;
        }

        public BusCompanyModel() { }

        public int pk { get; set; }
        public string companyName { get; set; }
        public string manager { get; set; }
        public string tel { get; set; }
        public string cell { get; set; }
        public string email { get; set; }


        public tblBusCompany ToDbModel()
        {
            return new tblBusCompany
            {
                pk = pk,
                companyName = companyName,
                manager = manager,
                tel = tel,
                cell = cell,
                email = email,
            };
        }
    }
}