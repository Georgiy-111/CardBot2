using CardBot2.Context;
using CardBot2.Constants;
using CardBot2.Services;
using CardBot2.UI;
using Telegram.Bot;

namespace CardBot2.Handlers.Commands;

/// <summary>
/// Обработчик команды /start.
/// 
/// Отвечает за:
/// - инициализацию диалога с пользователем
/// - перевод пользователя в состояние MainMenu
/// - отображение главного меню бота
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

    /// <summary>
    /// Команда, которую обрабатывает handler.
    /// </summary>
    public string Command => BotCommands.Start;

    /// <summary>
    /// Обработка команды /start.
    /// Устанавливает состояние пользователя и отправляет главное меню.
    /// </summary>
    public async Task HandleAsync(BotContext context)
    {
        // Устанавливаем состояние пользователя
        _userStateService.SetState(
            context.ChatId,
            Domain.UserState.MainMenu
        );

        // Отправляем приветственное сообщение и клавиатуру
        await _botClient.SendTextMessageAsync(
            chatId: context.ChatId,
            text: BotMessages.StartMessage,
            replyMarkup: BotKeyboards.MainMenu
        );
    }
}