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

        //здесь четко привяжем к DbView
        public string DbViewBindedName { get; set; }

        public List<string> PossibleKeys { get; set; }

        public List<DQFilter> Filters { get; set; }
        //public List<WildcardModel> Wildcards { get; set; }

        public List<DynEntity> Run() {
            throw new NotImplementedException();
        }
    }

    public class DynEntity {
        //public List<WildcardModel> FilledWildcards { get; set; }
    }
}
