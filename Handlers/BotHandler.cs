using CardBot2.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace CardBot2.Handlers;

public class BotHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly ICardService _cardService;

    public BotHandler(ITelegramBotClient botClient, ICardService cardService)
    {
        _botClient = botClient;
        _cardService = cardService;
    }

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
                    case "/start":
                        await SendStartMessage(message.Chat.Id);
                        break;
                    case "🃏 Вытянуть карту":
                        await SendRandomCard(message.Chat.Id);
                        break;
                }
            }

            await Task.Delay(500);
        }
    }

    private async Task SendStartMessage(long chatId)
    {
        var keyboard = new ReplyKeyboardMarkup(
            new[] { new KeyboardButton[] { "🃏 Вытянуть карту" } })
        {
            ResizeKeyboard = true,
            OneTimeKeyboard = false
        };

        await _botClient.SendTextMessageAsync(
            chatId: chatId,
            text: "Привет! Нажми кнопку чтобы вытянуть карту 🃏",
            replyMarkup: keyboard
        );
    }

    private async Task SendRandomCard(long chatId)
    {
        var card = _cardService.GetRandomCard();

        if (!System.IO.File.Exists(card.ImagePath))
        {
            await _botClient.SendTextMessageAsync(chatId, "Файл карты не найден");
            return;
        }

        await using var stream = System.IO.File.OpenRead(card.ImagePath);
        await _botClient.SendPhotoAsync(
            chatId: chatId,
            photo: new InputOnlineFile(stream, System.IO.Path.GetFileName(card.ImagePath)),
            caption: $"{card.Name}\n\n{card.Description}"
        );
    }
}
