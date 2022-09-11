using System.Collections.Concurrent;
using Polly;
using Polly.Timeout;
using PollySampleWebApplication.Configurations;

namespace PollySampleWebApplication.Services.Registries;

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

        return _policies.AddOrUpdate(serviceName, _ => Policy.TimeoutAsync(ConfigManager.Config.Polly.Timeout.TimeoutInSecond
                , TimeoutStrategy.Pessimistic)
            , (_, policy) => policy);
    }
}