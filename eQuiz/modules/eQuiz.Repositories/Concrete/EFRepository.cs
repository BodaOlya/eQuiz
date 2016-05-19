using eQuiz.Repositories.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace eQuiz.Repositories.Concrete
{
    public class EFRepository: BaseRepository, IRepository
    {
        #region Private Fields
        #endregion Private Fields

        #region Protected Properties 

        #endregion Protected Properties

        #region Constructors

        public EFRepository(IDataContextFactory dataContextFactory)
            : base(dataContextFactory)
        {            
        }

        #endregion Constructors

        #region IRepository Members

        public virtual TEntity GetByKey<TKey, TEntity>(string keyPropertyName, TKey key, params Expression<Func<TEntity, object>>[] paths) where TEntity : class
        {
            using (var context = this.DataContextFactory.NewInstance())
            {
                IQueryable<TEntity> query = context.Query<TEntity>();
                query = IncludePaths(paths, query);
                return query.Single(this.GetSelectByKeyCriteria<TKey, TEntity>(keyPropertyName, key));               
            }    
        }

        public virtual List<TEntity> Get<TEntity>(Func<TEntity, bool> criteria = null, params Expression<Func<TEntity, object>>[] paths) where TEntity: class
        {
            using (var context = this.DataContextFactory.NewInstance())
            {
                IQueryable<TEntity> query = context.Query<TEntity>();
                query = IncludePaths(paths, query);               
                if (criteria != null)
                {
                    query = query.Where(criteria).AsQueryable();
                }                
                return query.ToList();                
            }
        }

        public virtual TEntity GetSingle<TEntity>(Func<TEntity, bool> criteria = null, params Expression<Func<TEntity, object>>[] paths) where TEntity : class
        {
            using (var context = this.DataContextFactory.NewInstance())
            {
                IQueryable<TEntity> query = context.Query<TEntity>();
                query = IncludePaths(paths, query);
                if (criteria != null)
                {
                    query = query.Where(criteria).AsQueryable();
                }
                return query.SingleOrDefault();
            }
        }

        public virtual int Count<TEntity>(Func<TEntity, bool> criteria = null) where TEntity : class
        {
            using (var context = this.DataContextFactory.NewInstance())
            {
                IQueryable<TEntity> query = context.Query<TEntity>();                
                if (criteria != null)
                {
                    query = query.Where(criteria).AsQueryable();
                }
                return query.Count();
            }
        }

        public virtual bool Exists<TEntity>(Func<TEntity, bool> criteria = null) where TEntity : class
        {
            using (var context = this.DataContextFactory.NewInstance())
            {
                IQueryable<TEntity> query = context.Query<TEntity>();
                if (criteria != null)
                {
                    return query.Any(criteria);
                }
                else
                {
                    return query.Any();
                }                
            }
        }

        public virtual TEntity Update<TEntity>(TEntity entity) where TEntity : class
        {
            using (var context = this.DataContextFactory.NewInstance())
            {
                context.AttachModified(entity);
                context.SaveChanges();
                return entity;                
            }
        }

        public virtual TEntity Insert<TEntity>(TEntity entity) where TEntity : class
        {
            using (var context = this.DataContextFactory.NewInstance())
            {
                context.Insert(entity);
                context.SaveChanges();
                return entity;
            }
        }

        public virtual void Delete<TKey, TEntity>(string keyPropertyName, TKey key) where TEntity : class
        {
            using (var context = this.DataContextFactory.NewInstance())
            {
                TEntity entity = context.Query<TEntity>().Single(this.GetSelectByKeyCriteria<TKey, TEntity>(keyPropertyName, key));
                context.Delete(entity);
                context.SaveChanges();
            }
        }

        #endregion IRepository Members

        #region Protected Virtual Members       

        #endregion Protected Virtual Members

        #region Helpers
       
        #endregion Helpers
    }
}
