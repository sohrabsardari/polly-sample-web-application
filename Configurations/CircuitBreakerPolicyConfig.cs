namespace PollySampleWebApplication.Configurations;

public class CircuitBreakerPolicyConfig
{
    public double FailureThreshold { get; set; }
    public int SamplingDurationInSecond { get; set; }
    public int MinimumThroughput { get; set; }
    public int DurationOfBreakInSecond { get; set; }
}