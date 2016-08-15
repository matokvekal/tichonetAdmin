using System;
using System.Collections.Generic;

namespace Business_Logic.SqlContext {
    public interface ISqlLogic : IDisposable {
        /// <summary>
        /// table - name of table,
        /// fieldNames - array of column names,
        /// condition - raw T-SQL string that starts with "WHERE"
        /// </summary>
        IList<IDictionary<string, object>> FetchData(IEnumerable<string> fieldNames, string table, string schema = "dbo", string condition = null);
        
        /// <summary>
        /// returns dictionary:
        /// name: colomn name
        /// type: colomn SQL type
        /// </summary>
        IList<IDictionary<string, string>> GetColomnsInfos(string table, string schema = "dbo");
    }

}
