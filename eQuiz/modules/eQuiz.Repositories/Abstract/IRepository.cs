using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace eQuiz.Repositories.Abstract
{
    public interface IRepository
    {
        TEntity GetByKey<TKey, TEntity>(string keyPropertyName, TKey key, params Expression<Func<TEntity, object>>[] paths) where TEntity: class;
        List<TEntity> Get<TEntity>(Func<TEntity, bool> criteria = null, params Expression<Func<TEntity, object>>[] paths) where TEntity: class;
        TEntity GetSingle<TEntity>(Func<TEntity, bool> criteria = null, params Expression<Func<TEntity, object>>[] paths) where TEntity: class;
        int Count<TEntity>(Func<TEntity, bool> criteria = null) where TEntity : class;
        bool Exists<TEntity>(Func<TEntity, bool> criteria = null) where TEntity : class;
        TEntity Update<TEntity>(TEntity entity) where TEntity : class;
        TEntity Insert<TEntity>(TEntity entity) where TEntity : class;
        void Delete<TKey, TEntity>(string keyPropertyName, TKey key) where TEntity : class;
    }
}
