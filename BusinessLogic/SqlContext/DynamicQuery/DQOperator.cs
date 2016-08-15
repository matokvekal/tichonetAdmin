using System;
using System.Collections.Generic;

namespace Business_Logic.SqlContext.DynamicQuery {

    public struct DQOperator {
        public static IEnumerable<DQOperator> GetAllowedForSqlType (string type) {
            type = type.ToLower();
            switch (type) {
                case "int":
                    return ForNumber.Clone() as DQOperator[];
                case "nvarchar":
                    return ForString.Clone() as DQOperator[];
                case "date":
                case "datetime":
                    return ForDate.Clone() as DQOperator[];
                default:
                    return ForObject.Clone() as DQOperator[];
            }
        }

        public static readonly DQOperator[] ForObject = { OP.Equals, OP.NotEquals };
        public static readonly DQOperator[] ForString = { OP.Equals, OP.NotEquals, OP.LIKE };
        public static readonly DQOperator[] ForNumber = 
        {
            OP.Equals,
            OP.NotEquals,
            OP.Greater,
            OP.GreaterOrEqual,
            OP.Less,
            OP.LessOrEqual
        };
        public static readonly DQOperator[] ForDate = ForNumber;


        public string SQLString { get { return OPToString(val); } }
        public string ShortString { get { return OPToShortString(val); } }
        public OP Operator { get { return val; } }
        public int RawInt { get { return (int)val; } }

        public DQOperator(string NamedOrSQLString) {
            val = RawStringToOP(NamedOrSQLString);
        }

        public DQOperator(OP Operator) {
            val = Operator;
        }

        public DQOperator(int OperatorIndex) {
            if (!Enum.IsDefined(typeof(OP), OperatorIndex))
                throw new InvalidCastException();
            val = (OP)OperatorIndex;
        }

        public static implicit operator DQOperator (OP oper) {
            return new DQOperator(oper);
        }

        OP val;

        public enum OP {
            Equals = 1,
            NotEquals = 2,
            LIKE = 201,
            Greater = 101,
            Less = 102,
            GreaterOrEqual = 103,
            LessOrEqual = 104,
        }

        static string OPToString(OP op) {
            switch (op) {
                case OP.Equals:
                    return "=";
                case OP.NotEquals:
                    return "<>";
                case OP.Greater:
                    return ">";
                case OP.Less:
                    return "<";
                case OP.GreaterOrEqual:
                    return ">=";
                case OP.LessOrEqual:
                    return "<=";
                case OP.LIKE:
                    return "LIKE";
            }
            throw new ArgumentException("Invalid Enum value");
        }

        static string OPToShortString(OP op) {
            switch (op) {
                case OP.Equals:
                    return "eq";
                case OP.NotEquals:
                    return "noeq";
                case OP.Greater:
                    return "gr";
                case OP.Less:
                    return "le";
                case OP.GreaterOrEqual:
                    return "greq";
                case OP.LessOrEqual:
                    return "leeq";
                case OP.LIKE:
                    return "like";
            }
            throw new ArgumentException("Invalid Enum value");
        }

        static OP RawStringToOP(string oper) {
            oper = oper.ToLower();
            switch (oper) {
                case "=":
                case "equals":
                case "equal":
                case "eq":
                    return OP.Equals;
                case "<>":
                case "notequals":
                case "notequal":
                case "noeq":
                    return OP.NotEquals;
                case ">":
                case "greater":
                case "great":
                case "gr":
                    return OP.Greater;
                case "<":
                case "less":
                case "le":
                    return OP.Less;
                case ">=":
                case "greaterorequals":
                case "greaterorequal":
                case "greatoreq":
                case "greq":
                    return OP.GreaterOrEqual;
                case "lessorequals":
                case "lessorequal":
                case "lessoreq":
                case "leeq":
                case "<=":
                    return OP.LessOrEqual;
                case "like":
                    return OP.LIKE;
            }
            throw new ArgumentException("Invalid String value");
        }
    }
}
