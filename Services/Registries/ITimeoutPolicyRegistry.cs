using Polly.Timeout;

namespace PollySampleWebApplication.Services.Registries;

public interface ITimeoutPolicyRegistry
{
    AsyncTimeoutPolicy GetPolicyFor(string serviceName);
}