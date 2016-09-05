using Business_Logic.MessagesModule.DataObjects;
using Business_Logic.SqlContext;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.MessagesModule.EntitiesExtensions {
    public static class tblFilterHelper {
        /// <summary>
        /// u can pass here sqlLogic, if you use this method in cycle or/and already have opened one.
        /// if not, new sqlLogic instance will be opened.
        /// </summary>
        public static ValueOperatorPair[] GetValueOperatorPairs (tblFilter filter, ISqlLogic sqlLogic = null) {
            //try {
            if (filter.autoUpdatedList.HasValue && filter.autoUpdatedList.Value) {
                var ValsOps = sqlLogic.FetchDataDistinct(new[] { filter.Key }, filter.tblRecepientFilter.tblRecepientFilterTableName.Name)
                    .Select(x => new ValueOperatorPair(x[filter.Key].ToString(), "=", filter.Type)).ToArray();
                return ValsOps;
            }
            else {
                var vals = JsonConvert.DeserializeObject<string[]>(filter.ValuesJSON);
                var ops = JsonConvert.DeserializeObject<string[]>(filter.OperatorsJSON);
                var ValsOps = new ValueOperatorPair[vals.Length];
                for (int i = 0; i < vals.Length; i++) {
                    ValsOps[i] = new ValueOperatorPair(vals[i], ops[i], filter.Type);
                }
                return ValsOps;
            }
            //}
            //catch {
            //    return new ValueOperatorPair[1];
            //}
        }

    }
}
