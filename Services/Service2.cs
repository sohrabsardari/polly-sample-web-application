namespace PollySampleWebApplication.Services;

public class Service2 : IService
{
    public async Task<string> Get()
    {
        await Task.Delay(3000);
        return nameof(Service2);
    }

}