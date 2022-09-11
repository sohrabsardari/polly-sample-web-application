namespace PollySampleWebApplication.Configurations;

public static class ConfigManager
{
    public static Config Config { get; private set; }
    
    public static void SetConfig(Config config)
    {
        Config = config;
    }
}