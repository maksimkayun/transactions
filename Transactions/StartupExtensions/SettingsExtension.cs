using Common.Settings;

namespace Transactions.StartupExtensions;

public static class SettingsExtension
{
    public static AppSettings UseAppSettings(this WebApplicationBuilder builder)
    {
        var section = builder.Configuration.Get<AppSettings>();
        builder.Services.AddSingleton(section);
        builder.Services.AddOptions<AppSettings>();

        return section;
    }
}