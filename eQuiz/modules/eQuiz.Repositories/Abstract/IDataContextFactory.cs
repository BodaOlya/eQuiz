namespace eQuiz.Repositories.Abstract
{
    public interface IDataContextFactory
    {
        IDataContext NewInstance(bool explicitOpenConnection = false);
    }
}