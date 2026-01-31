using CardBot2.Context;

namespace CardBot2.Handlers.Commands;

/// <summary>
/// Интерфейс для команды бота.
/// </summary>
public interface ICommand
{
    /// <summary>
    /// Текст команды, который приходит от пользователя.
    /// </summary>
    string CommandText { get; }

    /// <summary>
    /// Действие команды.
    /// </summary>
    Task ExecuteAsync(BotContext context);
}