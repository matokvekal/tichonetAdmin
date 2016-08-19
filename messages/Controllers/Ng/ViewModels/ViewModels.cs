using System;
using System.Collections.Generic;
using System.Reflection;
using Business_Logic.MessagesContext;
using System.Linq.Expressions;
using Newtonsoft.Json;

namespace ticonet.Controllers.Ng.ViewModels {

    public interface INgViewModel {
        bool ng_JustCreated { get; set; }
        bool ng_ToDelete { get; set; }
        int Id { get; set; }
    }

    public class RFilterTableVM : INgViewModel {
        public static POCOReflector<tblRecepientFilterTableName, RFilterTableVM> tblRecepientFilterTableNamePR =
            POCOReflector<tblRecepientFilterTableName, RFilterTableVM>.Create(
                (o, m) => m.Id = o.Id,
                (o, m) => m.Name = o.Name,
                (o, m) => m.ReferencedTableName = o.ReferncedTableName
            );

        public int Id { get; set; }
        public string Name { get; set; }

        public string ReferencedTableName { get; set; }

        #region INgViewModel

        public bool ng_JustCreated { get; set; }
        public bool ng_ToDelete { get; set; }

        #endregion
    }

    public class RFilterVM  : INgViewModel {
        public static POCOReflector<tblRecepientFilter, RFilterVM> tblRecepientFilterPR =
            POCOReflector<tblRecepientFilter, RFilterVM>.Create(
                (o, m) => m.Id = o.Id,
                (o, m) => m.Name = o.Name,
                (o, m) => m.BaseTableId = o.tblRecepientFilterTableNameId
            );

        public int Id { get; set; }
        public string Name { get; set; }
        public int BaseTableId { get; set; }

        //for ng

        public FilterVM[] filters { get; set; }
        public WildcardVM[] wildcards { get; set; }
        public RecepientcardVM[] reccards { get; set; }

        #region INgViewModel

        public bool ng_JustCreated { get; set; }
        public bool ng_ToDelete { get; set; }

        #endregion

    }

    public class ValueOperatorPair {
        public object Value { get; set; }
        public string Operator { get; set; }

        public ValueOperatorPair() {

        }

        public ValueOperatorPair(string value, string oper, string sqltype) {
            Operator = oper;
            //TODO EXCEPTION HANDLE
            switch (sqltype) {
                case "int":
                    int i = 0;
                    Value = int.TryParse(value, out i) ? i : 0;
                    break;
                case "float":
                case "real":
                    float f = 0;
                    Value = float.TryParse(value, out f) ? f : 0;
                    break;
                case "date":
                case "datetime":
                    Value = TryParseDateTime(value);
                    break;
                default:
                    Value = value;
                    break;
            }
        }

        static DateTime TryParseDateTime(string val) {
            DateTime output;
            if (DateTime.TryParse(val, out output))
                return output;
            //TODO REMOVE
            //["/Date(1469998800000)/"]
            //  /Date(-62135596800000)/
            //  /Date(-6)/
            //  01234567890
            //  12345678901
            //  -6)/
            if (val.StartsWith("/Date")) {
                val = val.Substring(6, val.Length - 6);
                val = val.Substring(0, val.Length - 2);
                var l = long.Parse(val);
                return new DateTime(l);
            }
            return DateTime.Now;
        }
    }

    public class FilterVM : INgViewModel {
        public static POCOReflector<tblFilter, FilterVM> tblFilterPR =
            POCOReflector<tblFilter, FilterVM>.Create(
                (o, m) => m.Id = o.Id,
                (o, m) => m.Name = o.Name,
                (o, m) => m.RecepientFilterId = o.tblRecepientFilterId,
                (o, m) => m.Key = o.Key,
                //(o, m) => m.Value = JsonConvert.DeserializeObject<string[]>( o.Value ),
                //(o, m) => m.Operator = JsonConvert.DeserializeObject<string[]>( o.Operator ),
                (o, m) => {
                    //TODO REFACTOR
                    try {
                        var vals = JsonConvert.DeserializeObject<string[]>(o.Value);
                        var ops = JsonConvert.DeserializeObject<string[]>(o.Operator);
                        m.ValsOps = new ValueOperatorPair[vals.Length];
                        for (int i = 0; i < vals.Length; i++) {
                            m.ValsOps[i] = new ValueOperatorPair (vals[i],ops[i],o.Type);
                        }
                    }
                    catch {
                        m.ValsOps = new ValueOperatorPair[1];
                    }
                },
                (o, m) => m.Type = o.Type,
                (o, m) => m.allowMultipleSelection = o.allowMultipleSelection ?? false,
                (o, m) => m.allowUserInput = o.allowUserInput ?? false,
                (o, m) => m.autoUpdatedList = o.autoUpdatedList ?? false
            );

        public int Id { get; set; }
        public string Name { get; set; }
        public int RecepientFilterId { get; set; }
        public string Key { get; set; }
        public ValueOperatorPair[] ValsOps { get; set; }

        //public string[] Value { get; set; }
        //public string[] Operator { get; set; }
        public string Type {get;set;}

        public bool allowMultipleSelection { get; set; }
        public bool allowUserInput { get; set; }

        public bool autoUpdatedList { get; set; }

        #region INgViewModel

        public bool ng_JustCreated { get; set; }
        public bool ng_ToDelete { get; set; }

        #endregion
    }

    public class WildcardVM : INgViewModel {
        public static POCOReflector<tblWildcard, WildcardVM> tblWildcardPR =
            POCOReflector<tblWildcard, WildcardVM>.Create(
                (o, m) => m.Id = o.Id,
                (o, m) => m.RecepientFilterId = o.tblRecepientFilterId,
                (o, m) => m.Name = o.Name,
                (o, m) => m.Code = o.Code,
                (o, m) => m.Key = o.Key
            );

        public int Id { get; set; }
        public int RecepientFilterId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Key { get; set; }

        #region INgViewModel

        public bool ng_JustCreated { get; set; }
        public bool ng_ToDelete { get; set; }

        #endregion
    }

    public class RecepientcardVM : INgViewModel {
        public static POCOReflector<tblRecepientCard, RecepientcardVM> tblRecepientCardPR =
            POCOReflector<tblRecepientCard, RecepientcardVM>.Create(
                (o, m) => m.Id = o.Id,
                (o, m) => m.RecepientFilterId = o.tblRecepientFilterId,
                (o, m) => m.Name = o.Name,
                (o, m) => m.NameKey = o.NameKey,
                (o, m) => m.EmailKey = o.EmailKey,
                (o, m) => m.PhoneKey = o.PhoneKey
            );

        public int Id { get; set; }
        public int RecepientFilterId { get; set; }
        public string Name { get; set; }
        public string NameKey { get; set; }
        public string EmailKey { get; set; }
        public string PhoneKey { get; set; }

        #region INgViewModel

        public bool ng_JustCreated { get; set; }
        public bool ng_ToDelete { get; set; }

        #endregion
    }

    public class FilterValueContainer {
        public object[] Value { get; set; }
        public int FilterId { get; set; }
    }

    public class TemplateVM : INgViewModel {
        public static POCOReflector<tblTemplate, TemplateVM> tblTemplatePR =
            POCOReflector<tblTemplate, TemplateVM>.Create(
                (o, m) => m.Id = o.Id,
                (o, m) => m.RecepientFilterId = o.tblRecepientFilterId,
                (o, m) => m.Name = o.Name,
                (o, m) => m.IsSms = o.IsSms,
                (o, m) => m.MsgHeader = o.MsgHeader,
                (o, m) => m.MsgBody = o.MsgBody,
                (o, m) => {
                    if (o.FilterValueContainersJSON == null)
                        m.FilterValueContainers = new FilterValueContainer[] { new FilterValueContainer() };
                    else
                        m.FilterValueContainers = JsonConvert.DeserializeObject<FilterValueContainer[]>(o.FilterValueContainersJSON);
                }
            );

        public int Id { get; set; }
        public int RecepientFilterId { get; set; }

        public string Name { get; set; }
        public bool IsSms { get; set; }

        public string MsgHeader { get; set; }
        public string MsgBody { get; set; }
        public FilterValueContainer[] FilterValueContainers { get; set; }

        #region INgViewModel

        public bool ng_JustCreated { get; set; }
        public bool ng_ToDelete { get; set; }

        #endregion
    }

    //--------------------------------------------------

    public static class VMConstructor {
        public static TModel MakeFromObj<TOrig, TModel>(TOrig obj, POCOReflector<TOrig, TModel> reflector) where TModel : class, new() {
            var vm = Activator.CreateInstance<TModel>();
            //TODO EXCEPTION HANDLING
            reflector.Run(obj, vm);
            return vm;
        }

        public static PropertyInfo GetPropertyInfo<TSource, TProperty>
            (Expression<Func<TSource, TProperty>> propertySelector) {

            Type type = typeof(TSource);

            MemberExpression member = propertySelector.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' not refers to a property.",
                    propertySelector.ToString()));

            PropertyInfo propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    propertySelector.ToString()));

            return propInfo;
        }

        public static string GetPropertyName<TSource, TProperty>
            (Expression<Func<TSource, TProperty>> propertySelector) {
            return GetPropertyInfo(propertySelector).Name;
        }
    }

    public class POCOReflector<TOriginal, TModel> {
        public static POCOReflector<TOriginal, TModel> Create(params Action<TOriginal, TModel>[] funcs) {
            return new POCOReflector<TOriginal, TModel>(funcs);
        }

        Action<TOriginal, TModel>[] funcs;

        private POCOReflector (params Action<TOriginal, TModel>[] funcs) {
            this.funcs = funcs;
        }

        public void Run (TOriginal o, TModel m) {
            foreach (var f in funcs)
                f(o, m);
        }
    }

}