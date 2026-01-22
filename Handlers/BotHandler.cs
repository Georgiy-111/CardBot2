using CardBot2.Context;
using CardBot2.Handlers.Commands;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CardBot2.Handlers;

/// <summary>
/// Центральный обработчик сообщений Telegram-бота.
/// Делегирует выполнение команд соответствующим ICommandHandler.
/// </summary>
public class BotHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly IEnumerable<ICommandHandler> _handlers;

    public BotHandler(
        ITelegramBotClient botClient,
        IEnumerable<ICommandHandler> handlers)
    {
        _botClient = botClient;
        _handlers = handlers;
    }

    /// <summary>
    /// Обработка входящего текстового сообщения.
    /// </summary>
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
        }
        else
        {
            await _botClient.SendTextMessageAsync(
                chatId: context.ChatId,
                text: "Я тебя не понял 🙂 Нажми кнопку 🃏 Вытянуть карту.",
                replyMarkup: UI.BotKeyboards.MainMenu
            );
        }
    }
}