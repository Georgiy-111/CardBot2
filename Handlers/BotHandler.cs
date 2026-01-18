using CardBot2.Constants;
using CardBot2.Services;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

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

                switch (message.Text)
                {
                    case BotCommands.Start:
                        await SendStartMessage(message.Chat.Id);
                        break;

                    case BotCommands.DrawCard:
                        await SendRandomCard(message.Chat.Id);
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
    private async Task SendStartMessage(long chatId)
    {
        var keyboard = new ReplyKeyboardMarkup(
            new[]
            {
                new KeyboardButton[] { BotCommands.DrawCard }
            })
        {
            ResizeKeyboard = true,
            OneTimeKeyboard = false
        };

        await _botClient.SendTextMessageAsync(
            chatId: chatId,
            text: BotMessages.StartMessage,
            replyMarkup: keyboard
        );
    }

    /// <summary>
    /// Получает случайную карту и отправляет её пользователю.
    /// </summary>
    private async Task SendRandomCard(long chatId)
    {
        var card = _cardService.GetRandomCard();

        if (!System.IO.File.Exists(card.ImagePath))
        {
            await _botClient.SendTextMessageAsync(
                chatId,
                BotMessages.CardNotFound
            );
            return;
        }

        await using var stream = System.IO.File.OpenRead(card.ImagePath);

        await _botClient.SendPhotoAsync(
            chatId: chatId,
            photo: new InputOnlineFile(
                stream,
                System.IO.Path.GetFileName(card.ImagePath)
            ),
            caption: $"{card.Name}\n\n{card.Description}"
        );
    }
}
