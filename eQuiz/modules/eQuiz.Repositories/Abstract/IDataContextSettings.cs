using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eQuiz.Repositories.Abstract
{
    public interface IDataContextSettings
    {
        string ConnectionString { get; }
    }
}
