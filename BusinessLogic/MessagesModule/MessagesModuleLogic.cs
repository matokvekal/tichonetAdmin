using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Business_Logic.MessagesModule {

    public enum ItemSaveBehaviour {
        AddOnly = 1,
        UpdateOnly = 2,
        AllowAll = 10,
    }

    public class MessagesModuleLogic : MessagesModuleBaseLogic {

        //-------------------------------------------
        //CREATE
        //---------------------

        /// <summary>
        /// Creates Enity that tracked by EF
        /// </summary>
        public TEntity Create<TEntity>()
                        where TEntity : class, IMessagesModuleEntity {
            return DB.Set<TEntity>().Create();
        }


        //-------------------------------------------
        //GET
        //---------------------

        public List<TEntity> GetAll <TEntity>()
            where TEntity: class,IMessagesModuleEntity 
        {
            return DB.Set<TEntity>().ToList();
        }

        public TEntity Get<TEntity>(int Id)
                        where TEntity : class, IMessagesModuleEntity {
            return DB.Set<TEntity>().FirstOrDefault(x => x.Id == Id);
        }

        public IQueryable<TEntity> GetFilteredQueryable<TEntity>(IEnumerable<IQueryFilter> filters, IQueryable<TEntity> baseQuery = null)
                where TEntity : class, IMessagesModuleEntity {

            var query = baseQuery ?? DB.Set<TEntity>().AsQueryable();
            var entityType = typeof(TEntity);
            var entityTypeExpr = Expression.Parameter(entityType);
            if (filters == null)
                return query;
            filters = filters.Where(x => x.Valid);
            foreach (var filter in filters) {
                var propInfo = entityType.GetProperty(filter.key);
                if (propInfo != null) {
                    var leftExpr = Expression.Property(entityTypeExpr, propInfo);
                    var rightExpr = Expression.Convert(
                        Expression.Constant(ValidateToType(filter.val, propInfo.PropertyType)), 
                        propInfo.PropertyType);
                    var condition = Expression.Lambda<Func<TEntity, bool>>(
                        BuildExpressionByOperator(filter.op, leftExpr, rightExpr),
                        entityTypeExpr);
                    query = query.Where(condition);
                }
            }
            return query;
        }

        public List<TEntity> GetFiltered<TEntity>(int? Skip, int? Take, IEnumerable<IQueryFilter> filters, out int countWithoutTake, IQueryable<TEntity> baseQuery = null)
        where TEntity : class, IMessagesModuleEntity {

            var query = GetFilteredQueryable<TEntity>(filters, baseQuery);
            countWithoutTake = query.Count();
            if (Skip != null)
                query.Skip(Skip.Value);
            if (Take != null)
                query.Take(Take.Value);
            return query.ToList();
        }

        //-------------------------------------------
        //Save/Add/Update
        //---------------------

        public TEntity Save<TEntity> (TEntity item, ItemSaveBehaviour ISB = ItemSaveBehaviour.AllowAll)
                        where TEntity : class, IMessagesModuleEntity {
            var dbSet = DB.Set<TEntity>();
            var exsItem = Get<TEntity>(item.Id);
            if (exsItem != null) {
                if (ISB == ItemSaveBehaviour.AddOnly )
                    throw new InvalidOperationException("Attemption to add already existing item. change ItemSaveBehaviour if behaviour is intended");
                DB.Entry(item).State = EntityState.Modified;
            }
            else {
                if (ISB == ItemSaveBehaviour.UpdateOnly)
                    throw new InvalidOperationException("Attemption to save new item. change ItemSaveBehaviour if behaviour is intended");
                dbSet.Add(item);
            }
            DB.SaveChanges();
            return item;
        }

        /// <summary>
        /// Only Saving Existing Allowed
        /// </summary>
        public TEntity SaveChanges<TEntity>(TEntity item)
                        where TEntity : class, IMessagesModuleEntity {
            DB.Entry(item).State = EntityState.Modified;
            DB.SaveChanges();
            return item;
        }

        /// <summary>
        /// Only Adding New Allowed
        /// </summary>
        public TEntity Add<TEntity>(TEntity item)
                where TEntity : class, IMessagesModuleEntity {
            DB.Entry(item).State = EntityState.Added;
            DB.SaveChanges();
            return item;
        }

        /// <summary>
        /// Only Adding New Allowed
        /// </summary>
        public void AddRange<TEntity>(ICollection<TEntity> items)
                        where TEntity : class, IMessagesModuleEntity {
            DB.Set<TEntity>().AddRange(items);
            
            DB.SaveChanges();
        }

        //-------------------------------------------
        //Delete
        //---------------------

        public void Delete<TEntity> (TEntity item)
            where TEntity : class, IMessagesModuleEntity 
        {
            DB.Set<TEntity>().Remove(item);
            DB.SaveChanges();
        }

        public bool Delete<TEntity>(int Id)
            where TEntity : class, IMessagesModuleEntity 
        {
            var item = Get<TEntity>(Id);
            if (item == null) 
                return false;
            Delete(item);
            return true;
        }

        //-------------------------------------------
        //Private part
        //---------------------

        static MethodInfo StringContains_methodInfo = typeof(string).GetMethod("Contains");

        static Expression BuildExpressionByOperator(string Operator, MemberExpression property, Expression constant) {
            Operator = Operator == null ? string.Empty : Operator.ToLower();
            switch (Operator) {
                case "<>":
                case "notequals":
                case "notequal":
                case "noeq":
                    return Expression.NotEqual(property, constant);
                case ">":
                case "greater":
                case "great":
                case "gr":
                    return Expression.GreaterThan(property, constant);
                case "<":
                case "less":
                case "le":
                    return Expression.LessThan(property, constant);
                case ">=":
                case "greaterorequals":
                case "greaterorequal":
                case "greatoreq":
                case "greq":
                    return Expression.GreaterThanOrEqual(property, constant);
                case "lessorequals":
                case "lessorequal":
                case "lessoreq":
                case "leeq":
                case "<=":
                    return Expression.LessThanOrEqual(property, constant);
                case "like":
                case "contains":
                    return Expression.Call(property, StringContains_methodInfo, constant);
                case "=":
                case "==":
                case "===":
                case "equals":
                case "equal":
                case "eq":
                default:
                    return Expression.Equal(property, constant);
            }
        }

        Type dateNullableType = typeof(DateTime?);
        Type dateType = typeof(DateTime);

        object ValidateToType(object val, Type type) {
            //date time for some reason comes here as string....
            if (type == dateType || type == dateNullableType)
                return DateTime.Parse(val.ToString());
            return val;
        }
    }

}
