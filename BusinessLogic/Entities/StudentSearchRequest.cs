using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.Entities
{
    public class StudentSearchRequest
    {
        public string Name
        {
            get; set; 
            
        }
        public string Shicva { get; set; }
        public string Class { get; set; }
        public string Address { get; set; }

        public string Color { get; set; }
        public int Line { get; set; }
        public string SortColumn { get; set; }
        public string SortOrder { get; set; }

        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
