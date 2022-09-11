using Autofac.Extensions.DependencyInjection;

namespace PollySampleWebApplication
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webHostBuilder => {
                    webHostBuilder
                        .UseStartup<Startup>();
                })
                .Build();

            await host.RunAsync();
        }
    }
}