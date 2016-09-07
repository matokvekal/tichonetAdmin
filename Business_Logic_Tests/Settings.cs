using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Tests {
    public static class Settings {
        public const string MessageContextConnectionString =
            @"metadata=res://*/MessagesModule.MessageContext.csdl|res://*/MessagesModule.MessageContext.ssdl|res://*/MessagesModule.MessageContext.msl;
                        provider=System.Data.SqlClient;
                        provider connection string="";
                        data source =.;
                        initial catalog = BusProject;
                        integrated security = True;
                        MultipleActiveResultSets=True;
                        App=EntityFramework"";";

        public const string SqlConnectionString = "Data Source=.;Initial Catalog=BusProject;Connect Timeout=360;Integrated Security=True";
    }
}
