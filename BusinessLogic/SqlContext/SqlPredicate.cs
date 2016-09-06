using System;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using System.Text;

namespace Business_Logic.SqlContext {

    public class SqlPredicate {

        enum ptype {
            body,
            or,
            and
        }

        ptype predicateType;
        bool isEmpty;

        List<SqlPredicate> childNodes;

        string Key;
        SqlOperator op;
        object value;
        SqlType type;

        public static SqlPredicate BuildEndNode (string Key, SqlOperator op, object value, SqlType type) {
            if (string.IsNullOrWhiteSpace(Key) || value == null)
                throw new ArgumentNullException();

            var p = new SqlPredicate {
                predicateType = ptype.body,
                Key = Key,
                op = op,
                value = value,
                type = type
            };
            p.isEmpty = false;
            return p;
        }

        public static SqlPredicate BuildOrNode () {
            var p = new SqlPredicate();
            p.predicateType = ptype.or;
            p.childNodes = new List<SqlPredicate>();
            p.isEmpty = true;
            return p;
        }

        public static SqlPredicate BuildAndNode() {
            var p = new SqlPredicate();
            p.predicateType = ptype.and;
            p.childNodes = new List<SqlPredicate>();
            p.isEmpty = true;
            return p;
        }

        //TODO RECOURSION HANDLE
        public void Append (SqlPredicate predicate) {
            if (predicateType == ptype.body)
                throw new InvalidOperationException("You cant append predicates to EndNodes");
            if (predicate == null)
                throw new ArgumentNullException();
            isEmpty = false;
            childNodes.Add(predicate);
        }

        /// <summary>
        /// returns true if empty
        /// </summary>
        public bool CheckEmptiness() {
            RevalidateEmptiness();
            return isEmpty;
        }

        public string ToSqlString () {
            StringBuilder output = new StringBuilder();
            if (predicateType == ptype.body) {
                output.Append(Key);
                output.Append(" ");
                output.Append(op.SQLString);
                output.Append(" ");
                output.Append(SqlType.NetObjectToSqlQueryFormat(type,value));
            }
            else {
                if (childNodes.Count > 0) {
                    RevalidateEmptiness();
                    var conc = predicateType == ptype.and ? " AND " : " OR ";
                    childNodes.ForEach(x => {
                        if (x.isEmpty) return;
                        output.Append(" (");
                        output.Append(x.ToSqlString());
                        output.Append(") ");
                        output.Append(conc);
                    });
                    if (output.Length > conc.Length)
                        //it guaranties to us that at least one condition was written 
                        //to sql string 
                        //(because even if there are some childNodes,
                        //we should check here some way if there are all empty)
                        output.Remove(output.Length - conc.Length, conc.Length);
                }
            }
            return output.ToString();
        }

        protected void RevalidateEmptiness() {
            if (predicateType != ptype.body) {
                isEmpty = true;
                childNodes.ForEach(x => {
                    x.RevalidateEmptiness();
                    if (!x.isEmpty)
                        isEmpty = false;
                });
            }
        }

        private SqlPredicate() { }


    }

}
