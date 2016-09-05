using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.MessagesModule.DataObjects {
    public enum MessageType {
        Sms,
        Email
    }

    public class Message {
        public string Header { get; set; }
        public string Body { get; set; }
        public string Adress { get; set; }
        public MessageType Type { get; set; }
    }
}
