using Transactions.Settings;

namespace Transactions.StartupExtensions;

public static class SettingsExtension
{
    public static AppSettings UseAppSettings(this WebApplicationBuilder builder)
    {
        var section = builder.Configuration.Get<AppSettings>();
        builder.Services.AddSingleton(section);

        return section;
    }
}