namespace Habitus.Helpers;

public class AppSettings
{
    public string Secret { get; set; } = string.Empty;
    public string ValidAudience { get; set; } = string.Empty;
    public string ValidIssuer { get; set; } = string.Empty;
    public string TelegramToken { get; set; } = string.Empty;
    public string TelegramBotUsername { get; set; } = string.Empty;

}