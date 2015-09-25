namespace CiscoWLC.WebAuth.Client.Settings
{
    public static class SettingsValidator
    {
        public static bool AreValid(this MainSettings settings)
        {
            return !string.IsNullOrWhiteSpace(settings.ConnectionSettings.Ssid)
                   && !string.IsNullOrWhiteSpace(settings.LoginPageSettings.LoginPageUrl)
                   && !string.IsNullOrWhiteSpace(settings.AuthSettings.Username)
                   && !string.IsNullOrWhiteSpace(settings.AuthSettings.Password);
        }
    }
}