using CardBot2.Context;
using CardBot2.Constants;
using CardBot2.Domain;
using CardBot2.Services;
using Telegram.Bot;
using Telegram.Bot.Types.InputFiles;

namespace CardBot2.Handlers.Commands;

/// <summary>
/// Обработчик команды вытягивания карты.
/// </summary>
public class DrawCardCommandHandler : ICommandHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly ICardService _cardService;
    private readonly IUserStateService _userStateService;

    public DrawCardCommandHandler(
        ITelegramBotClient botClient,
        ICardService cardService,
        IUserStateService userStateService)
    {
        _botClient = botClient;
        _cardService = cardService;
        _userStateService = userStateService;
    }

    public string Command => BotCommands.DrawCard;

    public async Task HandleAsync(BotContext context)
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

        // Обновляем состояние пользователя
        _userStateService.SetState(context.ChatId, UserState.ViewingCard);

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