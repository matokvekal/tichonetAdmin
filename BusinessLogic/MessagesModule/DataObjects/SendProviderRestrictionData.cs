using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.MessagesModule.DataObjects {
    public class SendProviderRestrictionData {
        public int MaxMessagesInHour { get; set; }
        public int MaxMessagesInDay { get; set; }
    }
}
