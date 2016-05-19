using eQuiz.Repositories.Abstract;

namespace eQuiz.Repositories.Concrete
{
    public class EFDataContextFactory : IDataContextFactory
    {
        private readonly IDataContextSettings _settings;        

        public EFDataContextFactory(IDataContextSettings settings)
        {
            _settings = settings;
        }

        public IDataContext NewInstance(bool explicitOpenConnection = false)
        {
            return new EFDataContext(_settings, explicitOpenConnection);
        }
    }
}