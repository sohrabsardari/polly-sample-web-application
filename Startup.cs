using System.Reflection;
using Autofac;
using PollySampleWebApplication.Configurations;
using PollySampleWebApplication.Services;
using PollySampleWebApplication.Services.Decorators;
using PollySampleWebApplication.Services.Registries;

namespace PollySampleWebApplication;

public class Startup
{
    
    public Startup(IConfiguration configuration)
    {
        ConfigManager.SetConfig(configuration.Get<Config>());
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCors(z =>
        {
            z.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod());
        });
        services.AddControllers();
    }


    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseCors(options => options
            .AllowAnyHeader()
            .AllowAnyMethod()
            .SetIsOriginAllowed(host => true)
            .AllowCredentials());
        app.UseAuthentication();

        if (env.IsDevelopment())
            app.UseDeveloperExceptionPage();

        app.UseRouting();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

    }


    public void ConfigureContainer(ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
            .Where(a => typeof(IService).IsAssignableFrom(a) && !a.Name.Contains("Decorator"))
            .AsImplementedInterfaces()
            .InstancePerLifetimeScope();
        builder.RegisterDecorator<PollyDecorator, IService>();
        builder.RegisterType<CircuitBreakerPolicyRegistry>().As<ICircuitBreakerRegistry>().SingleInstance();
        builder.RegisterType<TimeoutPolicyRegistry>().As<ITimeoutPolicyRegistry>().SingleInstance();
        builder.RegisterType<BulkHeadRegistry>().As<IBulkHeadRegistry>().SingleInstance();
    }
}
