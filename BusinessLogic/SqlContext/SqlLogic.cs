using System;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using System.Text;

namespace Business_Logic.SqlContext{

    public class SqlLogic : ISqlLogic{
        public void Dispose(){
            GC.SuppressFinalize(this);
        }

        private readonly IDbConnectionFactory _dbConnectionFactory;

        public SqlLogic(IDbConnectionFactory dbConnectionFactory){
            _dbConnectionFactory = dbConnectionFactory;
        }

        //TODO EXCEPTIONS HANDLING

        /// <summary>
        /// table - name of table,
        /// fieldNames - array of column names,
        /// condition - raw T-SQL string that starts with "WHERE"
        /// </summary>
        public IList<IDictionary<string,object>> FetchData (IEnumerable<string> fieldNames, string table, string schema = "dbo", string condition = null) {
            string command = MakeCommand(false, fieldNames, table, schema, condition);
            return FetchData(command, reader => {
                var dict = new Dictionary<string, object>();
                foreach (var field in fieldNames)
                    dict.Add(field, reader[field]);
                return dict as IDictionary<string, object>;
            });
        }

        /// <summary>
        /// table - name of table,
        /// fieldNames - array of column names,
        /// condition - raw T-SQL string that starts with "WHERE"
        /// </summary>
        public IList<IDictionary<string, object>> FetchDataDistinct(IEnumerable<string> fieldNames, string table, string schema = "dbo", string condition = null) {
            string command = MakeCommand(true, fieldNames, table, schema, condition);
            return FetchData(command, reader => {
                var dict = new Dictionary<string, object>();
                foreach (var field in fieldNames)
                    dict.Add(field, reader[field]);
                return dict as IDictionary<string, object>;
            });
        }

        IList<TData> FetchData<TData> (string command, Func<IDataReader,TData> dataMaker) {
            var result = new List<TData>();
            using (SqlConnection dbConnection = (SqlConnection)_dbConnectionFactory.CreateConnection()) {
                SqlCommand comm = new SqlCommand();
                comm.Connection = dbConnection;
                comm.CommandType = CommandType.Text;
                comm.CommandText = command;
                using (SqlDataReader reader = comm.ExecuteReader()) {
                    while (reader.Read())
                        result.Add(dataMaker(reader));
                }
            }
            return result;
        }

        /// <summary>
        /// distinct - if true, only different values
        /// table - name of table,
        /// fieldNames - array of column names,
        /// condition - raw T-SQL string that starts with "WHERE"
        /// </summary>
        string MakeCommand (bool distinct, IEnumerable<string> fieldNames, string table, string schema = "dbo", string condition = null) {
            StringBuilder command = new StringBuilder();
            command.Append("SELECT ");
            if (distinct)
                command.Append("DISTINCT ");
            foreach (var field in fieldNames) {
                command.Append("[");
                command.Append(field);
                command.Append("]");
                command.Append(",");
            }
            command.Remove(command.Length - 1, 1);
            command.Append(" FROM ");
            command.Append("[");
            command.Append(schema);
            command.Append("].");
            command.Append("[");
            command.Append(table);
            command.Append("] ");
            command.Append(condition);
            return command.ToString();
        }

        /// <summary>
        /// returns dictionary:
        /// name: colomn name
        /// type: colomn SQL type
        /// </summary>
        public IList<IDictionary<string, string>> GetColomnsInfos(string table, string schema = "dbo") {
            string command = MakeCommand(false, new[] { "COLUMN_NAME", "DATA_TYPE" }, 
                "COLUMNS", "INFORMATION_SCHEMA", "WHERE TABLE_NAME='"+
                table+ "' AND TABLE_SCHEMA='"+schema+"'");
            return FetchData(command, x => {
                var dict = new Dictionary<string, string>();
                dict.Add("name", x.GetString(0));
                dict.Add("type", x.GetString(1));
                return dict as IDictionary<string, string>;
            });
        }

        /// <summary>
        /// returns:
        /// colomn SQL type
        /// </summary>
        public string GetColomnType(string table, string colomnName, string schema = "dbo") {
            string command = MakeCommand(false, new[] { "DATA_TYPE" },
                "COLUMNS", "INFORMATION_SCHEMA", "WHERE TABLE_NAME='" +
                table + "' AND COLUMN_NAME='" + colomnName +"' AND TABLE_SCHEMA='" + schema + "'");
            return FetchData(command, x => x.GetString(0))[0];
        }
    }
}
