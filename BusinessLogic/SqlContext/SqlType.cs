using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.SqlContext {

    public struct SqlType {

        public enum sqlt {
            NVARCHAR,
            INT,
            FLOAT,
            REAL,
            DATE,
            DATETIME,
            TIME,
            BIT
        }

        readonly sqlt _type;

        public SqlType(string sqlString) {
            _type = SqlStringToType(sqlString);
        }

        public SqlType(sqlt type) {
            _type = type;
        }

        public sqlt Type { get { return _type; } }
        public string SqlString { get { return SqlTypeString(_type); } }
        public int RawInt { get { return (int)_type; } }
        public Type NetType { get { return sqlToNetTypes[_type]; } }

        static string SqlTypeString (sqlt type) {
            switch (type) {
                case sqlt.NVARCHAR:
                    return "nvarchar";
                case sqlt.INT:
                    return "int";
                case sqlt.FLOAT:
                    return "float";
                case sqlt.REAL:
                    return "real";
                case sqlt.DATE:
                    return "date";
                case sqlt.DATETIME:
                    return "datetime";
                case sqlt.TIME:
                    return "time";
                case sqlt.BIT:
                    return "bit";
            }
            throw new ArgumentException();
        }

        static sqlt SqlStringToType(string type) {
            switch (type) {
                case "nvarchar":
                    return sqlt.NVARCHAR;
                case "int":
                    return sqlt.INT;
                case "float":
                    return sqlt.FLOAT;
                case "real":
                    return sqlt.REAL;
                case "date":
                    return sqlt.DATE;
                case "datetime":
                    return sqlt.DATETIME;
                case "time":
                    return sqlt.TIME;
                case "bit":
                    return sqlt.BIT;
            }
            throw new ArgumentException();
        }

        public static implicit operator string (SqlType type) {
            return type.SqlString;
        }

        public static implicit operator SqlType(string type) {
            return new SqlType(type);
        }

        public static implicit operator Type (SqlType type) {
            return type.NetType;
        }

        public static implicit operator sqlt (SqlType type) {
            return type._type;
        }

        static Dictionary<sqlt, Type> sqlToNetTypes = new Dictionary<sqlt, Type> {
            {sqlt.INT, typeof(int) },
            {sqlt.DATE, typeof(DateTime) },
            {sqlt.DATETIME, typeof(DateTime) },
            {sqlt.FLOAT, typeof(float) },
            {sqlt.NVARCHAR, typeof(string) },
            {sqlt.REAL, typeof(float) },
            {sqlt.TIME, typeof(TimeSpan) },
            {sqlt.BIT, typeof(bool) },
        };

        //CONVERTORS

        public static string NetObjectToSqlQueryFormat(SqlType type, object obj) {
            switch (type._type) {
                case sqlt.NVARCHAR:
                    return "N'" + obj.ToString().Replace("'", "''") + "'";
                case sqlt.INT:
                    return ((int)obj).ToString();
                case sqlt.FLOAT:
                case sqlt.REAL:
                    return ((float)obj).ToString();
                case sqlt.BIT:
                    bool value = false;
                    if (obj == null)
                        value = false;
                    else if (obj is string)
                        value = bool.Parse(obj.ToString());
                    else
                        value = (bool)obj;
                    return (value) ? "1" : "0";
                case sqlt.DATE:
                    return "N'" + ((DateTime)obj).ToString("yyyyMMdd") + "'";
                case sqlt.DATETIME:
                    return "N'" + ((DateTime)obj).ToString("yyyyMMdd HH:mm:ss.fff") + "'";
                case sqlt.TIME:
                    throw new NotImplementedException();
            }
            throw new ArgumentException();
        }

        public static object SqlStoreFormatToNetObject(SqlType type, string value) {
            switch (type._type) {
                case sqlt.NVARCHAR:
                    return value;
                case sqlt.INT:
                    int i = 0;
                    return int.TryParse(value, out i) ? i : 0;
                case sqlt.FLOAT:
                case sqlt.REAL:
                    float f = 0;
                    return float.TryParse(value, out f) ? f : 0;
                case sqlt.BIT:
                    bool val;
                    return bool.TryParse(value, out val) ? val : false;
                case sqlt.DATE:
                case sqlt.DATETIME:
                    return TryParseDateTime(value);
                case sqlt.TIME:
                    throw new NotImplementedException();
            }
            throw new ArgumentException();
        }

        static DateTime TryParseDateTime(string val) {
            DateTime output;
            if (DateTime.TryParse(val, out output))
                return output;
            if (val.StartsWith("/Date")) {
                val = val.Substring(6, val.Length - 6);
                val = val.Substring(0, val.Length - 2);
                var l = long.Parse(val);
                return new DateTime(l);
            }
            return DateTime.Now;
        }

    }
}
