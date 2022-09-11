using Polly;
using Polly.Wrap;
using PollySampleWebApplication.Services.Registries;

namespace PollySampleWebApplication.Services.Decorators;

public class PollyDecorator : IService
{
    private readonly AsyncPolicyWrap _policyWrap;
    private readonly IService _service;
    public PollyDecorator(IService service, ICircuitBreakerRegistry circuitBreakerRegistry
        , ITimeoutPolicyRegistry timeoutPolicyRegistry)
    {
        _service = service;
        var serviceName = _service.GetType().ToString();
        _policyWrap = Policy.WrapAsync(circuitBreakerRegistry.GetPolicyFor(serviceName), timeoutPolicyRegistry.GetPolicyFor(serviceName));
    }

    public async Task<string> Get()
    {
        return await _policyWrap.ExecuteAsync(async () => await _service.Get());
    }

}