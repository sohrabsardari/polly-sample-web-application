using System.Collections.Concurrent;
using Polly;
using Polly.CircuitBreaker;
using Polly.Timeout;
using PollySampleWebApplication.Configurations;

namespace PollySampleWebApplication.Services.Registries;

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
            var pollyConfig = ConfigManager.Config.Polly;
            return Policy
                .Handle<Exception>()
                .Or<TimeoutRejectedException>()
                .AdvancedCircuitBreakerAsync(
                    failureThreshold: pollyConfig.CircuitBreaker.FailureThreshold,
                    samplingDuration: TimeSpan.FromSeconds(pollyConfig.CircuitBreaker.SamplingDurationInSecond),
                    minimumThroughput: pollyConfig.CircuitBreaker.MinimumThroughput,
                    durationOfBreak: TimeSpan.FromSeconds(pollyConfig.CircuitBreaker.DurationOfBreakInSecond));
        }, (s, policy) => policy);
    }

    public List<ServiceHealth> GetServicesHealth()
    {
        return _policies.Select(a => new ServiceHealth(a.Key, a.Value.CircuitState.ToString())).ToList();
    }
}