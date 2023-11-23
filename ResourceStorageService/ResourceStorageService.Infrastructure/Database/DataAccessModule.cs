using Autofac;

namespace ResourceStorageService.Infrastructure.Database
{

    public class DataAccessModule : Autofac.Module
    {
        private readonly string _mongoConnection;
        public DataAccessModule(string mongoConnection)
        {            
            _mongoConnection = mongoConnection;
        }

        protected override void Load(ContainerBuilder builder)
        {          
            builder.RegisterType<MongoConnection>()
             .As<IMongoConnection>()
             .WithParameter("connection", _mongoConnection)
             .InstancePerLifetimeScope();

        }
    }
}
