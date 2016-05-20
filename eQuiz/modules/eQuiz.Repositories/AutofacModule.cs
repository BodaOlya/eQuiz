using Autofac;
using eQuiz.Repositories.Abstract;
using eQuiz.Repositories.Concrete;

namespace eQuiz.Repositories
{
    public class AutofacModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EFDataContextFactory>().As<IDataContextFactory>();
            builder.RegisterType<EFRepository>().As<IRepository>();                                   
        }
    }
}
