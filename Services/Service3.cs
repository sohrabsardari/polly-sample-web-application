namespace PollySampleWebApplication.Services;

public class Service3 : IService
{
    public async Task<string> Get()
    {
        await Task.Delay(15000);
        return nameof(Service3);
    }
}