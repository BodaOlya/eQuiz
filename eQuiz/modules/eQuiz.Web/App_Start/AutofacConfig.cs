using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;

namespace eQuiz.Web
{
    public static class AutofacConfig
    {
        public static void Initialize()
        {
            var builder = new ContainerBuilder();            

            builder.RegisterModule<eQuiz.Web.AutofacModule>();            
            builder.RegisterModule<eQuiz.Repositories.AutofacModule>();

            //builder.Register(c => ServiceLocator.Current).As<IServiceLocator>();

            var container = builder.Build();            
            
            //ServiceLocator.SetLocatorProvider(() => new AutofacServiceLocator(container));
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}