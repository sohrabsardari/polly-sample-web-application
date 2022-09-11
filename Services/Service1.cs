namespace PollySampleWebApplication.Services;

public class Service1 : IService
{
    public async Task<string> Get()
    {
        await Task.Delay(50);
        return nameof(Service1);
    }
}