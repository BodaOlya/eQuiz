using System.Collections.Generic;
using System.Linq;
using eQuiz.Repositories.Abstract;
using System.Data.Entity;

namespace eQuiz.Repositories.Concrete
{
    public class EFDataContext : IDataContext
    {
        #region Fields

        private readonly IDataContextSettings _settings;
        private readonly string _connectionString;
        private readonly bool _explicitOpenConnection;        
        private DbContext _efDbContext = null;

        #endregion

        #region Constructors

        public EFDataContext(IDataContextSettings settings, bool explicitOpenConnection = false)
        {
            this._settings = settings;
            this._connectionString = this._settings.ConnectionString;
            this._explicitOpenConnection = explicitOpenConnection;            
        }

        #endregion        

        #region IObjectContextRepository

        public IQueryable<TEntity> Query<TEntity>() where TEntity : class
        {            
            return EFDbContext.Set<TEntity>();
        }

        public void SaveChanges()
        {
            EFDbContext.SaveChanges();
        }

        public void Insert<TEntity>(TEntity entity) where TEntity : class
        {            
            EFDbContext.Set<TEntity>().Add(entity);
        }

        public void AttachModified<TEntity>(TEntity entity) where TEntity : class
        {
            EFDbContext.Set<TEntity>().Attach(entity);
            EFDbContext.Entry(entity).State = EntityState.Modified;
        }

        public void InsertEach<TEntity>(IEnumerable<TEntity> entitys) where TEntity : class
        {
            foreach (TEntity entity in entitys)
            {
                EFDbContext.Set<TEntity>().Add(entity);
            }
        }

        public void Delete<TEntity>(TEntity entity) where TEntity : class
        {
            EFDbContext.Set<TEntity>().Remove(entity);
        }

        public void DeleteEach<TEntity>(IEnumerable<TEntity> entitys) where TEntity : class
        {
            foreach (TEntity entity in entitys)
            {
                EFDbContext.Set<TEntity>().Remove(entity);
            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (this._efDbContext != null)
            {
                this._efDbContext.Dispose();
            }
        }

        #endregion

        #region Helpers

        private DbContext CreateObjectContext()
        {
            DbContext context = new DbContext(this._connectionString);
            if (this._explicitOpenConnection)
            {
                if (context.Database.Connection.State != System.Data.ConnectionState.Open)
                {
                    context.Database.Connection.Open();
                }
            }
            return context;
        }

        private DbContext EFDbContext
        {
            get
            {
                if (this._efDbContext == null)
                {
                    this._efDbContext = CreateObjectContext();
                }
                return this._efDbContext;
            }
        }

        #endregion

    }
}
