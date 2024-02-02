using Habitus.Helpers;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Habitus.Services;
public class TelegramService : IHostedService
{
    private readonly AppSettings _appSettings;
    private readonly ITelegramBotClient _botClient;
    private CancellationTokenSource _cts;

    public TelegramService(AppSettings appSettings)
    {
        _appSettings = appSettings;
        _botClient = new TelegramBotClient(appSettings.TelegramToken);
        _cts = new CancellationTokenSource();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        StartBot();
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await StopBotAsync();
    }

    public void StartBot()
    {
        Task.Run(() => RunBotAsync(_cts.Token));
    }

    public async Task StopBotAsync()
    {
        _cts.Cancel();
        await Task.Delay(1000);
    }

    private async Task RunBotAsync(CancellationToken cancellationToken)
    {
        var me = await _botClient.GetMeAsync();
       
        Console.WriteLine($"Start listening for @{me.Username}");

        ReceiverOptions receiverOptions = new()
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        _botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cancellationToken
        );

        await Task.Delay(Timeout.Infinite, cancellationToken);
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        // Only process Message updates: https://core.telegram.org/bots/api#message
        if (update.Message is not { } message)
            return;
        // Only process text messages
        if (message.Text is not { } messageText)
            return;

        if (message.Text == "/start")
        {
            var chatId = message.Chat.Id;
            var response = $"Your code is: {chatId}. Copy and paste it back into Habitus for confirmation.";
            DateTime messageTime = DateTime.Now;
            await SendMessage(chatId, response);
        }

    }

    public async Task ScheduleMessage(string? messageText, long chatId)
    {
        await SendMessage(chatId, messageText);
    }

    public async Task SendMessage(long chatId, string? messageText)
    {
        Message sentMessage = await _botClient.SendTextMessageAsync(
           chatId: chatId,
           text: messageText,
           cancellationToken: _cts.Token);
    }

    private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }

}
