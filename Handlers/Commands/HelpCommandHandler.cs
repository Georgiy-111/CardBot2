using CardBot2.Constants;
using CardBot2.Context;
using CardBot2.UI;
using Telegram.Bot;

namespace CardBot2.Handlers.Commands;

/// <summary>
/// Обработчик команды "ℹ️ Как это работает".
/// </summary>
public class HelpCommandHandler : ICommandHandler
{
    private readonly ITelegramBotClient _botClient;

    public HelpCommandHandler(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }

    public string Command => BotCommands.Help;

    public async Task HandleAsync(BotContext context)
    {
        await _botClient.SendTextMessageAsync(
            chatId: context.ChatId,
            text: BotMessages.HelpMessage,
            replyMarkup: BotKeyboards.MainMenu
        );
    }
}