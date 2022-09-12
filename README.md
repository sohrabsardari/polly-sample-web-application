# Polly sample code in dotnet core web application

Implementing Circuitbreaker, bulkhead and timeout using polly in dotnet core web application using decorator and autofac.
## Define Registries
```c#
public interface ICircuitBreakerRegistry
{
    AsyncCircuitBreakerPolicy GetPolicyFor(string serviceName);
    List<ServiceHealth> GetServicesHealth();
}
```
```c#
public interface IBulkHeadRegistry
{
    AsyncBulkheadPolicy GetPolicyFor(string serviceName);
}
```
```c#
public interface ITimeoutPolicyRegistry
{
    AsyncTimeoutPolicy GetPolicyFor(string serviceName);
}
```
## Implementation of the registeries
```c#
public class CircuitBreakerPolicyRegistry : ICircuitBreakerRegistry
{
    private readonly ConcurrentDictionary<string, AsyncCircuitBreakerPolicy> _policies =
        new ConcurrentDictionary<string, AsyncCircuitBreakerPolicy>();
    public AsyncCircuitBreakerPolicy GetPolicyFor(string serviceName)
    {
        if (_policies.ContainsKey(serviceName))
        {
            return _policies[serviceName];
        }

        return _policies.AddOrUpdate(serviceName, _ =>
        {
            return Policy
                .Handle<Exception>()
                .Or<TimeoutRejectedException>()
                .AdvancedCircuitBreakerAsync(
                    failureThreshold: 0.5,
                    samplingDuration: TimeSpan.FromSeconds(10),
                    minimumThroughput: 8,
                    durationOfBreak: TimeSpan.FromSeconds(30));
        }, (s, policy) => policy);
    }

    public List<ServiceHealth> GetServicesHealth()
    {
        return _policies.Select(a => new ServiceHealth(a.Key, a.Value.CircuitState.ToString())).ToList();
    }
}
```
```c#
public class BulkHeadRegistry : IBulkHeadRegistry
{
    private readonly ConcurrentDictionary<string, AsyncBulkheadPolicy> _policies =
        new ConcurrentDictionary<string, AsyncBulkheadPolicy>();
    public AsyncBulkheadPolicy GetPolicyFor(string serviceName)
    {
        if (_policies.ContainsKey(serviceName))
        {
            return _policies[serviceName];
        }

        return _policies.AddOrUpdate(serviceName, a => Policy.BulkheadAsync(15, 50)
            , (_, policy) => policy);
    }
}
```
```c#
public class TimeoutPolicyRegistry : ITimeoutPolicyRegistry
{
    private readonly ConcurrentDictionary<string, AsyncTimeoutPolicy> _policies =
        new ConcurrentDictionary<string, AsyncTimeoutPolicy>();
    public AsyncTimeoutPolicy GetPolicyFor(string serviceName)
    {
        if (_policies.ContainsKey(serviceName))
        {
            return _policies[serviceName];
        }

        return _policies.AddOrUpdate(serviceName, _ => Policy.TimeoutAsync(10, TimeoutStrategy.Pessimistic)
            , (_, policy) => policy);
    }
}
```
## Define a decorator for IService
```c#
public class PollyDecorator : IService
{
    private readonly AsyncPolicyWrap _policyWrap;
    private readonly IService _service;
    public PollyDecorator(IService service, ICircuitBreakerRegistry circuitBreakerRegistry
        , ITimeoutPolicyRegistry timeoutPolicyRegistry, IBulkHeadRegistry bulkHeadRegistry)
    {
        _service = service;
        var serviceName = _service.GetType().ToString();
        _policyWrap = Policy.WrapAsync(
            circuitBreakerRegistry.GetPolicyFor(serviceName),
            bulkHeadRegistry.GetPolicyFor(serviceName),
            timeoutPolicyRegistry.GetPolicyFor(serviceName));
    }

    public async Task<string> Get()
    {
        return await _policyWrap.ExecuteAsync(async () => await _service.Get());
    }
}
```
```c#
public class Service1 : IService
{
    public async Task<string> Get()
    {
        await Task.Delay(15000);
        return nameof(Service1);
    }
}
```
## Bootstrapping in Autofac
```c#
 builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
     .Where(a => typeof(IService).IsAssignableFrom(a) && !a.Name.Contains("Decorator"))
     .AsImplementedInterfaces()
     .InstancePerLifetimeScope();
 builder.RegisterDecorator<PollyDecorator, IService>();
 builder.RegisterType<CircuitBreakerPolicyRegistry>().As<ICircuitBreakerRegistry>().SingleInstance();
 builder.RegisterType<TimeoutPolicyRegistry>().As<ITimeoutPolicyRegistry>().SingleInstance();
 builder.RegisterType<BulkHeadRegistry>().As<IBulkHeadRegistry>().SingleInstance();
```
