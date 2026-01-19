using CardBot2.Context;

namespace CardBot2.Handlers.Commands;

/// <summary>
/// Общий интерфейс для всех обработчиков команд бота.
/// </summary>
public interface ICommandHandler
{
    /// <summary>
    /// Команда, которую обрабатывает handler (например, /start или 🃏 Вытянуть карту)
    /// </summary>
    string Command { get; }

    /// <summary>
    /// Обрабатывает входящее сообщение в контексте.
    /// </summary>
    Task HandleAsync(BotContext context);
}