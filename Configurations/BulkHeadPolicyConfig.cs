namespace PollySampleWebApplication.Configurations;

public class BulkHeadPolicyConfig
{
    public int MaxParallelization { get; set; }
    public int MaxQueuingActions { get; set; }
}