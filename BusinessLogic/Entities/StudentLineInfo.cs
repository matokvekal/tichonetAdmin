using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace Business_Logic.Entities
{
    public class StudentLineInfo
    {
        public int Id { get; set; }
        public string Color { get; set; }
        public string Number { get; set; }
        public string Name { get; set; }
        public int Direction { get; set; }

        public DateTime? Date { get; set; }
    }
}
