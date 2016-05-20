using Autofac;
using Autofac.Integration.Mvc;
using eQuiz.Repositories.Abstract;
using eQuiz.Repositories.Concrete;
using System.Configuration;


namespace eQuiz.Web
{
	public class AutofacModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterControllers(System.Reflection.Assembly.GetExecutingAssembly());

            string connectionString = ConfigurationManager.ConnectionStrings["eQuizDB"].ConnectionString;
            builder.Register(с => new DefaultDataContextSettings(connectionString)).As<IDataContextSettings>();            
        }
    }
}