using CardBot2.Context;
using CardBot2.Constants;
using CardBot2.Services;
using Telegram.Bot;
using Telegram.Bot.Types.InputFiles;

namespace CardBot2.Handlers.Commands;

public class DrawCardCommandHandler : ICommandHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly ICardService _cardService;

    public DrawCardCommandHandler(ITelegramBotClient botClient, ICardService cardService)
    {
        _botClient = botClient;
        _cardService = cardService;
    }

    public string Command => BotCommands.DrawCard;

    public async Task HandleAsync(BotContext context)
    {
        var card = _cardService.GetRandomCard();

        if (!System.IO.File.Exists(card.ImagePath))
        {
            await _botClient.SendTextMessageAsync(context.ChatId, BotMessages.CardNotFound);
            return;
        }

        await using var stream = System.IO.File.OpenRead(card.ImagePath);

        await _botClient.SendPhotoAsync(
            chatId: context.ChatId,
            photo: new InputOnlineFile(stream, System.IO.Path.GetFileName(card.ImagePath)),
            caption: $"{card.Name}\n\n{card.Description}"
        );
    }
}