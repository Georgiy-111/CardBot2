using CardBot2.Context;
using CardBot2.Constants;
using CardBot2.UI;
using Telegram.Bot;

namespace CardBot2.Handlers.Commands;

public class StartCommandHandler : ICommandHandler
{
    private readonly ITelegramBotClient _botClient;

    public StartCommandHandler(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }

    public string Command => BotCommands.Start;

    public async Task HandleAsync(BotContext context)
    {
        await _botClient.SendTextMessageAsync(
            chatId: context.ChatId,
            text: BotMessages.StartMessage,
            replyMarkup: BotKeyboards.MainMenu
        );
    }
}