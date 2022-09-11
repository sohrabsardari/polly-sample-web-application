using System.Collections.Concurrent;
using Polly;
using Polly.Bulkhead;
using PollySampleWebApplication.Configurations;

namespace PollySampleWebApplication.Services.Registries;

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

        return _policies.AddOrUpdate(serviceName, a => Policy.BulkheadAsync(ConfigManager.Config.Polly.BulkHead.MaxParallelization,ConfigManager.Config.Polly.BulkHead.MaxQueuingActions)
            , (_, policy) => policy);
    }
}