using System;
using System.Collections.Generic;

namespace Business_Logic.SqlContext {
    public interface ISqlLogic : IDisposable {
        /// <summary>
        /// table - name of table,
        /// fieldNames - array of column names,
        /// condition - SqlPredicate Graph
        /// </summary>
        IList<IDictionary<string, object>> FetchData(IEnumerable<string> fieldNames, string table, string schema = "dbo", SqlPredicate condition = null);

        /// <summary>
        /// table - name of table,
        /// fieldNames - array of column names,
        /// condition - SqlPredicate Graph
        /// </summary>
        IList<IDictionary<string, object>> FetchDataDistinct(IEnumerable<string> fieldNames, string table, string schema = "dbo", SqlPredicate condition = null);

        /// <summary>
        /// returns dictionary:
        /// name: colomn name
        /// type: colomn SQL type
        /// </summary>
        IList<IDictionary<string, string>> GetColomnsInfos(string table, string schema = "dbo");

        /// <summary>
        /// returns:
        /// colomn SQL type
        /// </summary>
        string GetColomnType(string table, string colomnName, string schema = "dbo");
    }

}
