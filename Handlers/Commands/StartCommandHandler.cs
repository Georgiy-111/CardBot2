using CardBot2.Constants;
using CardBot2.Context;
using CardBot2.Domain;
using CardBot2.Services;
using CardBot2.UI;
using Telegram.Bot;

namespace CardBot2.Handlers.Commands;

/// <summary>
/// Обработчик команды /start.
/// </summary>
public class StartCommandHandler : ICommandHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly IUserStateService _userStateService;

    public StartCommandHandler(
        ITelegramBotClient botClient,
        IUserStateService userStateService)
    {
        _botClient = botClient;
        _userStateService = userStateService;
    }

    public string Command => BotCommands.Start;

    public async Task HandleAsync(BotContext context)
    {
        // Устанавливаем состояние пользователя
        _userStateService.SetState(context.ChatId, UserState.MainMenu);

        await _botClient.SendTextMessageAsync(
            chatId: context.ChatId,
            text: BotMessages.StartMessage,
            replyMarkup: BotKeyboards.MainMenu
        );
    }
}