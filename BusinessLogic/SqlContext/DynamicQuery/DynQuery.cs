using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Logic.SqlContext;

namespace Business_Logic.SqlContext.DynamicQuery {

    public class DQFilter {
        public int Id { get; set; }

        public string Key { get; set; }
        public string Val { get; set; }
        public string Op { get; set; }
    }

    public class DynQuery {
        public int Id { get; set; }

        public string DbViewBindedName { get; set; }

        public List<string> PossibleKeys { get; set; }

        public List<DQFilter> Filters { get; set; }
        //public List<WildcardModel> Wildcards { get; set; }

        public List<DynEntity> Run() {
            throw new NotImplementedException();
        }
    }

    /*
        DateTime myDateTime = DateTime.Now;
        string sqlFormattedDate = myDateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
     */

    public class DynEntity {
        //public List<WildcardModel> FilledWildcards { get; set; }
    }
}
