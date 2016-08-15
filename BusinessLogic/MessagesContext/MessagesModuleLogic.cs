using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Business_Logic.MessagesContext {

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
                        where TEntity : class, IMessagesContextEntity {
            return DB.Set<TEntity>().Create();
        }


        //-------------------------------------------
        //GET
        //---------------------

        public List<TEntity> GetAll <TEntity>()
            where TEntity: class,IMessagesContextEntity 
        {
            return DB.Set<TEntity>().ToList();
        }

        public TEntity Get<TEntity>(int Id)
                        where TEntity : class, IMessagesContextEntity {
            return DB.Set<TEntity>().FirstOrDefault(x => x.Id == Id);
        }

        public List<TEntity> GetFiltered<TEntity>(int? Skip, int? Take, IQueryFilter[] filters, out int countWithoutTake)
                        where TEntity : class, IMessagesContextEntity {
            //TODO Getting entity filtered is not covers all cases yet (types)
            //TODO AVOID DATA MOVE IN MEMORY
            var query = DB.Set<TEntity>().ToArray().AsEnumerable();
            foreach(var filter in filters) {
                var propInfo = typeof(TEntity).GetProperty(filter.key);
                if (propInfo != null) {
                    switch (filter.op) {
                        //TODO Getting entity filtered is not covers all cases yet (operators)
                        default:
                            //TODO filter.val is a boxed valuse type, that is why straight check will fail
                            query = query.Where(x => propInfo.GetValue(x).ToString() == filter.val.ToString());
                            break;
                    }
                }
            }
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
                        where TEntity : class, IMessagesContextEntity {
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

        public TEntity SaveChanges<TEntity>(TEntity item)
                        where TEntity : class, IMessagesContextEntity {
            DB.Entry(item).State = EntityState.Modified;
            DB.SaveChanges();
            return item;
        }

        /// <summary>
        /// Only Adding New Allowed
        /// </summary>
        public TEntity Add<TEntity>(TEntity item)
                where TEntity : class, IMessagesContextEntity {
            DB.Entry(item).State = EntityState.Added;
            DB.SaveChanges();
            return item;
        }

        /// <summary>
        /// Only Adding New Allowed
        /// </summary>
        public void AddRange<TEntity>(ICollection<TEntity> items)
                        where TEntity : class, IMessagesContextEntity {
            DB.Set<TEntity>().AddRange(items);
            
            DB.SaveChanges();
        }

        //-------------------------------------------
        //Delete
        //---------------------

        public void Delete<TEntity> (TEntity item)
            where TEntity : class, IMessagesContextEntity 
        {
            DB.Set<TEntity>().Remove(item);
            DB.SaveChanges();
        }

        public bool Delete<TEntity>(int Id)
            where TEntity : class, IMessagesContextEntity 
        {
            var item = Get<TEntity>(Id);
            if (item == null) 
                return false;
            Delete(item);
            return true;
        }
    }

}
