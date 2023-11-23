using Autofac.Extensions.DependencyInjection;

namespace ResourceStorageService.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                      .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                      .ConfigureWebHostDefaults(webHostBuilder =>
                      {
                          webHostBuilder
                      .UseIISIntegration()
                      .UseStartup<Startup>();
                      })
                      .Build();

            host.Run();
        }
    }
}