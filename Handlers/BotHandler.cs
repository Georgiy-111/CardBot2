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
/// Отвечает за:
/// - приём входящих сообщений (Message)
/// - определение команды пользователя
/// - делегирование обработки соответствующему ICommandHandler
/// - fallback-логику для неизвестных сообщений
///   с учётом текущего состояния пользователя
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

    /// <summary>
    /// Точка входа обработки входящего сообщения от Telegram.
    /// </summary>
    public async Task HandleAsync(Message message)
    {
        // Нас интересуют только текстовые сообщения
        if (message.Text == null)
            return;

        // Контекст инкапсулирует Message и даёт удобный доступ
        // к ChatId, тексту сообщения и др.
        var context = new BotContext(message);

        // Ищем обработчик, который соответствует команде пользователя
        var handler = _handlers.FirstOrDefault(
            h => h.Command == context.MessageText
        );

        // Если команда найдена — делегируем обработку
        if (handler != null)
        {
            await handler.HandleAsync(context);
            return;
        }

        // Если команда не распознана — применяем fallback-логику
        // с учётом состояния пользователя
        await HandleUnknownMessage(context);
    }

    /// <summary>
    /// Обработка неизвестных сообщений.
    ///
    /// Поведение зависит от текущего состояния пользователя:
    /// - в меню или после карты — мягко возвращаем к кнопкам
    /// - в остальных случаях — предлагаем начать с /start
    /// </summary>
    private async Task HandleUnknownMessage(BotContext context)
    {
        var state = _userStateService.GetState(context.ChatId);

        switch (state)
        {
            case UserState.MainMenu:
            case UserState.ViewingCard:
                await _botClient.SendTextMessageAsync(
                    chatId: context.ChatId,
                    text: $"Я тебя не понял 🙂 Нажми кнопку {BotCommands.DrawCard} или {BotCommands.Help}.",
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
    /// <summary>
    /// Отправляет сообщение с главным меню бота.
    /// Используется для fallback-логики и ситуаций,
    /// когда нужно гарантированно показать клавиатуру.
    /// </summary>
    private async Task SendMainMenuAsync(long chatId, string text)
    {
        await _botClient.SendTextMessageAsync(
            chatId: chatId,
            text: text,
            replyMarkup: BotKeyboards.MainMenu
        );
    }
}