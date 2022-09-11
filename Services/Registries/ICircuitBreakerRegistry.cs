using Polly.CircuitBreaker;

namespace PollySampleWebApplication.Services.Registries;

public interface ICircuitBreakerRegistry
{
    AsyncCircuitBreakerPolicy GetPolicyFor(string serviceName);
    List<ServiceHealth> GetServicesHealth();
}