using Autofac;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.OpenApi.Models;
using ResourceStorageService.API.AppConfig;
using ResourceStorageService.API.Swagger;
using ResourceStorageService.Infrastructure.Database;

namespace ResourceStorageService.API
{
    public class Startup
    {        
        public IConfiguration _configuration { get; }

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void ConfigureContainer(ContainerBuilder builder)
        {            
            var mongoConnection = _configuration.GetSection("MongoConnection").Value;

            builder.RegisterModule(new DataAccessModule(mongoConnection));
        }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddCors();
            services.AddControllers(config =>
            {
                config.EnableEndpointRouting = false;
            }).AddFluentValidation(fv =>
            {
                fv.ImplicitlyValidateRootCollectionElements = true;
            });

            services.AddHttpContextAccessor();
            services.AddHttpClient();

            services.AddMediatR(typeof(Startup));
            services.RegisterApplicationServices(_configuration);

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Resource Storage Service", Description = "Resource Storage Service to manage AWS files based on configured region", Version = "v1" });
                c.SchemaFilter<ResourceDataSet>();
            });
            services.AddMemoryCache();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Resource Storage Service");
            });

            app.UseCors(options => options.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
