using System;
using System.Collections.Generic;
using System.Reflection;
using Business_Logic.MessagesContext;
using System.Linq.Expressions;

namespace ticonet.Controllers.Ng.ViewModels {

    public class RFilterTableVM {
        public static List<OVmBinding<tblRecepientFilterTableName, RFilterTableVM>> tblRecepientFilterTableNameBND
            = new List<OVmBinding<tblRecepientFilterTableName, RFilterTableVM>> {
                OVmBinding<tblRecepientFilterTableName, RFilterTableVM>.Create( o => o.Id, m => m.Id),
                OVmBinding<tblRecepientFilterTableName, RFilterTableVM>.Create( o => o.Name, m => m.Name),
                OVmBinding<tblRecepientFilterTableName, RFilterTableVM>.Create( o => o.ReferncedTableName, m => m.ReferencedTableName)
            };

        public int Id { get; set; }
        public string Name { get; set; }
        public string ReferencedTableName { get; set; }
    }

    public class RFilterVM {
        public static List<OVmBinding<tblRecepientFilter, RFilterVM>> tblRecepientFilterBND
            = new List<OVmBinding<tblRecepientFilter, RFilterVM>> {
                OVmBinding<tblRecepientFilter, RFilterVM>.Create( o => o.Id, m => m.Id),
                OVmBinding<tblRecepientFilter, RFilterVM>.Create( o => o.Name, m => m.Name),
                OVmBinding<tblRecepientFilter, RFilterVM>.Create( o => o.tblRecepientFilterTableNameId, m => m.BaseTableId),
            };

        public int Id { get; set; }
        public string Name { get; set; }
        public int BaseTableId { get; set; }

        //for ng

        public FilterVM[] filters { get; set; }
        public FilterVM[] newfilters { get; set; }
        public FilterVM[] removedfilters { get; set; }

        public WildcardVM[] wildcards { get; set; }
        public WildcardVM[] newwildcards { get; set; }
        public WildcardVM[] removedwildcards { get; set; }

    }

    public class FilterVM {
        public static List<OVmBinding<tblFilter, FilterVM>> tblFilterBND
            = new List<OVmBinding<tblFilter, FilterVM>> {
                OVmBinding<tblFilter, FilterVM>.Create(o => o.Id, m => m.Id),
                OVmBinding<tblFilter, FilterVM>.Create(o => o.tblRecepientFilterId, m => m.RecepientFilterId),
                OVmBinding<tblFilter, FilterVM>.Create(o => o.Key, m => m.Key),
                OVmBinding<tblFilter, FilterVM>.Create(o => o.Value, m => m.Value),
                OVmBinding<tblFilter, FilterVM>.Create(o => o.Operator, m => m.Operator),
                OVmBinding<tblFilter, FilterVM>.Create(o => o.Type, m => m.Type)
            };

        public int Id { get; set; }
        public int RecepientFilterId { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string Operator { get; set; }
        public string Type {get;set;}
    }

    public class WildcardVM {
        public static List<OVmBinding<tblWildcard, WildcardVM>> tblWildcardBND
            = new List<OVmBinding<tblWildcard, WildcardVM>> {
                OVmBinding<tblWildcard, WildcardVM>.Create( o => o.Id, m => m.Id),
                OVmBinding<tblWildcard, WildcardVM>.Create( o => o.tblRecepientFilterId, m => m.RecepientFilterId),
                OVmBinding<tblWildcard, WildcardVM>.Create( o => o.Name, m => m.Name),
                OVmBinding<tblWildcard, WildcardVM>.Create( o => o.Code, m => m.Code),
                OVmBinding<tblWildcard, WildcardVM>.Create( o => o.Key, m => m.Key),
            };

        public int Id { get; set; }
        public int RecepientFilterId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Key { get; set; }
    }

    public class TemplateVM {
        public static List<OVmBinding<tblTemplate, TemplateVM>> tblTemplateBND
            = new List<OVmBinding<tblTemplate, TemplateVM>> {
                OVmBinding<tblTemplate, TemplateVM>.Create( o => o.Id, m => m.Id),
                OVmBinding<tblTemplate, TemplateVM>.Create( o => o.tblRecepientFilterId, m => m.RecepientFilterId),
                OVmBinding<tblTemplate, TemplateVM>.Create( o => o.Name, m => m.Name),
                OVmBinding<tblTemplate, TemplateVM>.Create( o => o.IsSms, m => m.IsSms),
                OVmBinding<tblTemplate, TemplateVM>.Create( o => o.MsgHeader, m => m.MsgHeader),
                OVmBinding<tblTemplate, TemplateVM>.Create( o => o.MsgBody, m => m.MsgBody),
            };

        public int Id { get; set; }
        public int RecepientFilterId { get; set; }

        public string Name { get; set; }
        public bool IsSms { get; set; }

        public string MsgHeader { get; set; }
        public string MsgBody { get; set; }
    }

    //--------------------------------------------------

    public static class VMConstructor {
        public static TModel MakeFromObj<TOrig,TModel> (TOrig obj, List<OVmBinding<TOrig, TModel>> bindings) where TModel : class, new() {
            var vm = Activator.CreateInstance<TModel>();
            //TODO EXCEPTION HANDLING
            foreach (var binding in bindings) 
                binding.Evaluate(obj, vm);
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

    public class OVmBinding<TOriginal, TModel> {
        public static OVmBinding<TOriginal, TModel> Create (string origProp, string modelProp, Func<object, object> Reflector = null) {
            var OP = typeof(TOriginal).GetProperty(origProp);
            var MP = typeof(TModel).GetProperty(modelProp);
            return new OVmBinding<TOriginal, TModel>(OP, MP, Reflector);
        }

        public static OVmBinding<TOriginal, TModel> Create(PropertyInfo origProp, PropertyInfo modelProp, Func<object, object> Reflector = null) {
            return new OVmBinding<TOriginal, TModel>(origProp, modelProp, Reflector);
        }

        public static OVmBinding<TOriginal, TModel> Create <TOriginalProp, TModelProp> 
            (Expression<Func<TOriginal, TOriginalProp>> origPropertySelector,
            Expression<Func<TModel, TModelProp>> modelPropertySelector,
            Func<object, object> Reflector = null) 
        {
            var origProp = VMConstructor.GetPropertyInfo(origPropertySelector);
            var modelProp = VMConstructor.GetPropertyInfo(modelPropertySelector);
            return new OVmBinding<TOriginal, TModel>(origProp, modelProp, Reflector);
        }

        private Func<object, object> straigthReflect = (x) => x;

        private OVmBinding(PropertyInfo origProp, PropertyInfo modelProp, Func<object, object> Reflector = null) {
            Original = origProp;
            Model = modelProp;
            if (Reflector == null)
                this.Reflector = straigthReflect;
            else
                this.Reflector = Reflector;
        }

        public readonly PropertyInfo Original;
        public readonly PropertyInfo Model;
        public readonly Func<object, object> Reflector;

        public void Evaluate (TOriginal original, TModel model) {
            var ev = Reflector(Original.GetValue(original));
            Model.SetValue(model, ev);
        }
    }

}