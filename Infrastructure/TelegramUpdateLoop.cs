using CardBot2.Handlers;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace CardBot2.Infrastructure;

/// <summary>
/// Отвечает ТОЛЬКО за получение обновлений от Telegram (long polling).
/// 
/// НЕ содержит бизнес-логики.
/// НЕ знает, какие команды есть у бота.
/// Его задача — получить Update и передать дальше.
/// </summary>
public class TelegramUpdateLoop
{
    private readonly ITelegramBotClient _botClient;
    private readonly BotHandler _botHandler;

    public TelegramUpdateLoop(
        ITelegramBotClient botClient,
        BotHandler botHandler)
    {
        _botClient = botClient;
        _botHandler = botHandler;
    }

    /// <summary>
    /// Запускает бесконечный цикл получения обновлений от Telegram.
    /// </summary>
    public async Task RunAsync()
    {
        int offset = 0;

        while (true)
        {
            var updates = await _botClient.GetUpdatesAsync(offset);

            foreach (var update in updates)
            {
                offset = update.Id + 1;

                // Нас интересуют только сообщения
                if (update.Type != UpdateType.Message)
                    continue;

                await _botHandler.HandleAsync(update.Message);
            }

            // Небольшая задержка, чтобы не спамить Telegram API
            await Task.Delay(500);
        }
    }
}