namespace PollySampleWebApplication.Configurations;

public class PollyConfig
{
    public CircuitBreakerPolicyConfig CircuitBreaker { get; set; }
    public TimeoutPolicyConfig Timeout { get; set; }
    public BulkHeadPolicyConfig BulkHead { get; set; }
}