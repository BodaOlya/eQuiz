using eQuiz.Repositories.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eQuiz.Repositories.Concrete
{
    public class DefaultDataContextSettings : IDataContextSettings
    {
        #region Fields

        private readonly string _connectionString;

        #endregion

        #region Constructors

        public DefaultDataContextSettings(string connectionString)
        {
            _connectionString = connectionString;
        }

        #endregion

        #region IDataContextSettings

        public string ConnectionString
        {
            get
            {
                return _connectionString;
            }
        }

        #endregion

    }
}
