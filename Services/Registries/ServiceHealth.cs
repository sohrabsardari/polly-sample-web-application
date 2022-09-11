namespace PollySampleWebApplication.Services.Registries;

public class ServiceHealth
{
    public string ServiceName { get; private set; }
    public string Status { get; private set; }

    public ServiceHealth(string serviceName, string status)
    {
        ServiceName = serviceName;
        Status = status;
    }
}