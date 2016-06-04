using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.Helpers
{
    public class StringHelper
    {
        public static string FixDecimalSeparator(string data)
        {
            string sep = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
           return data.Replace(".", sep).Replace(",", sep);
          
        }
    }
}
