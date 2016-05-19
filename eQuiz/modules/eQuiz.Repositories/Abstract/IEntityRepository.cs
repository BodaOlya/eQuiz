using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace eQuiz.Repositories.Abstract
{
    public interface IEntityRepository<TKey, TEntity> where TEntity : class
    {
        TEntity Get(TKey key, Expression<Func<TEntity, object>>[] paths = null);
        List<TEntity> Get(Func<TEntity, bool> criteria = null, Expression<Func<TEntity, object>>[] paths = null);
        TEntity Update(TEntity entity);
        TEntity Insert(TEntity entity);
        void Delete(TKey key);
    }
}
