using CardBot2.Constants;
using CardBot2.Context;
using CardBot2.Domain;
using CardBot2.Handlers.Commands;
using CardBot2.Services;
using CardBot2.UI;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CardBot2.Handlers;

/// <summary>
/// Центральный диспетчер сообщений Telegram-бота.
/// 
/// - принимает Message
/// - определяет команду
/// - делегирует обработку ICommandHandler
/// - реагирует на неизвестные сообщения с учётом состояния пользователя
/// </summary>
public class BotHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly IEnumerable<ICommandHandler> _handlers;
    private readonly IUserStateService _userStateService;

    public BotHandler(
        ITelegramBotClient botClient,
        IEnumerable<ICommandHandler> handlers,
        IUserStateService userStateService)
    {
        _botClient = botClient;
        _handlers = handlers;
        _userStateService = userStateService;
    }

    public async Task HandleAsync(Message message)
    {
        if (message.Text == null)
            return;

        var context = new BotContext(message);

        var handler = _handlers.FirstOrDefault(
            h => h.Command == context.MessageText
        );

        if (handler != null)
        {
            await handler.HandleAsync(context);
            return;
        }

        // Если команда не распознана — реагируем по состоянию
        await HandleUnknownMessage(context);
    }

    private async Task HandleUnknownMessage(BotContext context)
    {
        var state = _userStateService.GetState(context.ChatId);

        switch (state)
        {
            case UserState.MainMenu:
            case UserState.ViewingCard:
                await _botClient.SendTextMessageAsync(
                    chatId: context.ChatId,
                    text: $"Я тебя не понял 🙂 Нажми кнопку {BotCommands.DrawCard}.",
                    replyMarkup: BotKeyboards.MainMenu
                );
                break;

            default:
                await _botClient.SendTextMessageAsync(
                    chatId: context.ChatId,
                    text: "Напиши /start чтобы начать."
                );
                break;
        }
    }
}