using FluentValidation;
using MediatR;
using ResourceStorageService.Infrastructure.Database;
using ResourceStorageService.Infrastructure.DBContext;
using System.Reflection;

namespace ResourceStorageService.API.AppConfig
{
    public static class AppConfig
    {
        public static void RegisterApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            RegisterAppServices(services);
            RegisterDB(services, configuration);
        }

        private static void RegisterAppServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(IMediator), typeof(Mediator));
            services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddValidatorsFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly())
                .AddAutoMapper(Assembly.GetExecutingAssembly())
                .AddMediatR(Assembly.GetExecutingAssembly());

            services.AddTransient(typeof(ResourceStorageContext));
        }

        private static void RegisterDB(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient(typeof(IMongoConnection), typeof(MongoConnection));
        }
    }
}
