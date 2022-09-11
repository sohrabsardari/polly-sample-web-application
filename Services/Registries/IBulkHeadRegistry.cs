using Polly.Bulkhead;

namespace PollySampleWebApplication.Services.Registries;

public interface IBulkHeadRegistry
{
    AsyncBulkheadPolicy GetPolicyFor(string serviceName);
}