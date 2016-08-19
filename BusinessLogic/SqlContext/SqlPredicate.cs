﻿using System;
using System.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using System.Text;

namespace Business_Logic.SqlContext {

    public class SqlPredicate {

        public bool IsEmpty { get { return isEmpty; } }

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

        public string ToSqlString () {
            StringBuilder output = new StringBuilder();
            if (predicateType == ptype.body) {
                output.Append(Key);
                output.Append(" ");
                output.Append(op.SQLString);
                output.Append(" ");
                output.Append(type.NetObjectToSqlString(value));
            }
            else {
                if (childNodes.Count > 0) {
                    var conc = predicateType == ptype.and ? " AND " : " OR ";
                    childNodes.ForEach(x => {
                        if (x.isEmpty) return;
                        output.Append(" (");
                        output.Append(x.ToSqlString());
                        output.Append(") ");
                        output.Append(conc);
                    });
                    output.Remove(output.Length - conc.Length, conc.Length);
                }
            }
            return output.ToString();
        }

        private SqlPredicate() { }


    }

}
