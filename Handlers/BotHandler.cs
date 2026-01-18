using CardBot2.Constants;
using CardBot2.Services;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using CardBot2.Context;
using CardBot2.UI;

namespace CardBot2.Handlers;

/// <summary>
/// Центральный обработчик Telegram-бота.
/// 
/// Отвечает за:
/// - получение обновлений от Telegram API
/// - разбор входящих сообщений
/// - маршрутизацию команд (/start, вытянуть карту)
/// 
/// НЕ содержит бизнес-логики (как выбирается карта),
/// она вынесена в сервисы.
/// </summary>
public class BotHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly ICardService _cardService;

    public BotHandler(ITelegramBotClient botClient, ICardService cardService)
    {
        _botClient = botClient;
        _cardService = cardService;
    }

    /// <summary>
    /// Основной цикл получения и обработки обновлений от Telegram.
    /// Работает через long polling.
    /// </summary>
    public async Task HandleUpdatesAsync()
    {
        int offset = 0;

        while (true)
        {
            var updates = await _botClient.GetUpdatesAsync(offset);

            foreach (var update in updates)
            {
                offset = update.Id + 1;

                if (update.Type != UpdateType.Message)
                    continue;

                var message = update.Message;
                if (message?.Text == null)
                    continue;

                var context = new BotContext(message);

                switch (context.MessageText)
                {
                    case BotCommands.Start:
                        await SendStartMessage(context);
                        break;

                    case BotCommands.DrawCard:
                        await SendRandomCard(context);
                        break;
                }
            }

            // Небольшая задержка, чтобы не спамить Telegram API
            await Task.Delay(500);
        }
    }

    /// <summary>
    /// Отправляет стартовое сообщение и клавиатуру с кнопкой.
    /// </summary>
    private async Task SendStartMessage(BotContext context)
    {
        await _botClient.SendTextMessageAsync(
            chatId: context.ChatId,
            text: BotMessages.StartMessage,
            replyMarkup: BotKeyboards.MainMenu
        );
    }

    /// <summary>
    /// Получает случайную карту и отправляет её пользователю.
    /// </summary>
    private async Task SendRandomCard(BotContext context)
    {
        var card = _cardService.GetRandomCard();

        if (!System.IO.File.Exists(card.ImagePath))
        {
            await _botClient.SendTextMessageAsync(
                context.ChatId,
                BotMessages.CardNotFound
            );
            return;
        }

        await using var stream = System.IO.File.OpenRead(card.ImagePath);

        await _botClient.SendPhotoAsync(
            chatId: context.ChatId,
            photo: new InputOnlineFile(
                stream,
                System.IO.Path.GetFileName(card.ImagePath)
            ),
            caption: $"{card.Name}\n\n{card.Description}"
        );
    }
}
