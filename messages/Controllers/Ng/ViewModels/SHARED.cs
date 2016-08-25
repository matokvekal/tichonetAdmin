using System;
using System.Reflection;
using Business_Logic.MessagesModule;
using System.Linq.Expressions;
using Newtonsoft.Json;
using Business_Logic.SqlContext;

namespace ticonet.Controllers.Ng.ViewModels {

    public interface INgViewModel {
        bool ng_JustCreated { get; set; }
        bool ng_ToDelete { get; set; }
        int Id { get; set; }
    }

    /// <summary>
    /// This is object serves ONLY exchanging data with client.
    /// DO NOT use it some-where else
    /// </summary>
    public class ValueOperatorPair {
        public object Value { get; set; }
        public string Operator { get; set; }

        /// <summary>
        /// DONT USE THIS CONSTRUCTOR. Its for model-creator
        /// </summary>
        public ValueOperatorPair() {

        }

        public ValueOperatorPair(string value, string oper, string sqltype) {
            Operator = oper;
            //TODO EXCEPTION HANDLE
            Value = SqlType.SqlStoreFormatToNetObject(sqltype, value);
        }

    }

    /// <summary>
    /// This is object serves ONLY exchanging data with client.
    /// DO NOT use it some-where else
    /// </summary>
    public class FilterValueContainer {
        public object[] Values { get; set; }
        public int FilterId { get; set; }

        /// <summary>
        /// DONT USE THIS CONSTRUCTOR. Its for model-creator
        /// </summary>
        public FilterValueContainer (){

        }
    }

    //--------------------------------------------------

    public static class PocoConstructor {
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