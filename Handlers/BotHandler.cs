using CardBot2.Constants;
using CardBot2.Context;
using CardBot2.Domain;
using CardBot2.Services;
using CardBot2.UI;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace CardBot2.Handlers;

/// <summary>
/// Центральный обработчик сообщений Telegram-бота.
/// 
/// Обрабатывает входящие сообщения с учётом
/// текущего состояния пользователя.
/// </summary>
public class BotHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly ICardService _cardService;
    private readonly IUserStateService _userStateService;

    public BotHandler(
        ITelegramBotClient botClient,
        ICardService cardService,
        IUserStateService userStateService)
    {
        _botClient = botClient;
        _cardService = cardService;
        _userStateService = userStateService;
    }

    /// <summary>
    /// Основная точка входа обработки одного сообщения.
    /// </summary>
    public async Task HandleAsync(Message message)
    {
        if (message.Text == null)
            return;

        var context = new BotContext(message);
        var state = _userStateService.GetState(context.ChatId);

        switch (state)
        {
            case UserState.None:
            case UserState.MainMenu:
                await HandleMainMenuState(context);
                break;

            case UserState.ViewingCard:
                await HandleViewingCardState(context);
                break;

            default:
                await SendUnknownCommand(context);
                break;
        }
    }

    /// <summary>
    /// Обработка сообщений в состоянии главного меню.
    /// </summary>
    private async Task HandleMainMenuState(BotContext context)
    {
        switch (context.MessageText)
        {
            case BotCommands.Start:
                _userStateService.SetState(context.ChatId, UserState.MainMenu);
                await SendStartMessage(context);
                break;

            case BotCommands.DrawCard:
                _userStateService.SetState(context.ChatId, UserState.ViewingCard);
                await SendRandomCard(context);
                break;

            default:
                await SendUnknownCommand(context);
                break;
        }
    }

    /// <summary>
    /// Обработка сообщений после показа карты.
    /// </summary>
    private async Task HandleViewingCardState(BotContext context)
    {
        switch (context.MessageText)
        {
            case BotCommands.DrawCard:
                await SendRandomCard(context);
                break;

            case BotCommands.Start:
                _userStateService.SetState(context.ChatId, UserState.MainMenu);
                await SendStartMessage(context);
                break;

            default:
                await SendUnknownCommand(context);
                break;
        }
    }

    private async Task SendStartMessage(BotContext context)
    {
        await _botClient.SendTextMessageAsync(
            chatId: context.ChatId,
            text: BotMessages.StartMessage,
            replyMarkup: BotKeyboards.MainMenu
        );
    }

    private async Task SendRandomCard(BotContext context)
    {
        var card = _cardService.GetRandomCard();

        if (!System.IO.File.Exists(card.ImagePath))
        {
            await _botClient.SendTextMessageAsync(
                chatId: context.ChatId,
                text: BotMessages.CardNotFound
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

    private async Task SendUnknownCommand(BotContext context)
    {
        await _botClient.SendTextMessageAsync(
            chatId: context.ChatId,
            text: BotMessages.UnknownCommand,
            replyMarkup: BotKeyboards.MainMenu
        );
    }
}

