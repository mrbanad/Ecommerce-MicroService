using Microsoft.Extensions.Configuration;

namespace CommonServiceLibrary.Setting;

public static class ProjectSettings
{
    static ProjectSettings()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        DatabaseSetting = configuration.GetSection("ProjectSettings:DatabaseSetting").Get<DatabaseSetting>() ?? new DatabaseSetting();
        RabbitMqSetting = configuration.GetSection("ProjectSettings:RabbitMqSetting").Get<RabbitMqSetting>() ?? new RabbitMqSetting();
        ApiSetting = configuration.GetSection("ProjectSettings:ApiSetting").Get<ApiSetting>() ?? new ApiSetting();
    }

    public static DatabaseSetting DatabaseSetting { get; private set; }

    public static RabbitMqSetting RabbitMqSetting { get; private set; }

    public static ApiSetting ApiSetting { get; private set; }
}