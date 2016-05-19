using eQuiz.Repositories.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace eQuiz.Repositories.Concrete
{
    public class EFEntityRepository<TKey, TEntity>: BaseRepository, IEntityRepository<TKey, TEntity> where TEntity: class                                                                                 
    {
        #region Private Fields

        private string _keyPropertyName;

        #endregion Private Fields

        #region Protected Properties 

        #endregion Protected Properties

        #region Constructors

        public EFEntityRepository(IDataContextFactory dataContextFactory, string keyPropertyName):base(dataContextFactory)
        {
            this._keyPropertyName = keyPropertyName;
        }

        #endregion Constructors

        #region IRepository Members

        public virtual TEntity Get(TKey key, Expression<Func<TEntity, object>>[] paths = null)
        {
            using (var context = this.DataContextFactory.NewInstance())
            {
                IQueryable<TEntity> query = context.Query<TEntity>();
                query = IncludePaths(paths, query);
                return query.Single(this.GetSelectByKeyCriteria<TKey, TEntity>(this._keyPropertyName, key));               
            }    
        }

        public virtual List<TEntity> Get(Func<TEntity, bool> criteria = null, Expression<Func<TEntity, object>>[] paths = null)
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

        public virtual TEntity Update(TEntity entity)
        {
            using (var context = this.DataContextFactory.NewInstance())
            {
                context.AttachModified(entity);
                context.SaveChanges();
                return entity;                
            }
        }

        public virtual TEntity Insert(TEntity entity)
        {
            using (var context = this.DataContextFactory.NewInstance())
            {
                context.Insert(entity);
                context.SaveChanges();
                return entity;
            }
        }

        public virtual void Delete(TKey key)
        {
            using (var context = this.DataContextFactory.NewInstance())
            {
                TEntity entity = context.Query<TEntity>().Single(this.GetSelectByKeyCriteria<TKey, TEntity>(this._keyPropertyName, key));
                context.Delete(entity);
                context.SaveChanges();
            }
        }

        #endregion IRepository Members

        #region Protected Virtual Members       

        #endregion Protected Virtual Members

        #region Helpers

        ///// <summary>
        ///// Creates lambda expression: r => r.Key == key
        ///// The purpose of the expression is to select an entity by its key
        ///// </summary>
        ///// <param name="key"></param>
        ///// <returns></returns>
        //private Expression<Func<TEntity, bool>> GetSelectByKeyCriteria(TKey key)
        //{
        //    var entity = Expression.Parameter(typeof(TEntity), "r");

        //    var keyProperty = Expression.Property(entity, this._keyPropertyName);

        //    var keyValue = Expression.Constant(key);

        //    var equal = Expression.Equal(keyProperty, keyValue);

        //    var lambda = Expression.Lambda<Func<TEntity, bool>>(equal, entity);

        //    return lambda;
        //}

        //private IQueryable<TEntity> IncludePaths(Expression<Func<TEntity, object>>[] paths, IQueryable<TEntity> query)
        //{
        //    if (paths != null)
        //    {
        //        foreach (var path in paths)
        //        {
        //            query = query.Include(path);
        //        }
        //    }
        //    return query;
        //}


        #endregion Helpers

    }
}
