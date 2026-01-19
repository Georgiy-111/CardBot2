using CardBot2.Constants;
using Telegram.Bot.Types.ReplyMarkups;

namespace CardBot2.UI;

/// <summary>
/// Фабрика клавиатур Telegram-бота.
/// 
/// Отвечает ТОЛЬКО за:
/// - создание ReplyKeyboardMarkup
/// - конфигурацию кнопок
/// 
/// Не содержит логики бота.
/// </summary>
public static class BotKeyboards
{
    /// <summary>
    /// Главная клавиатура бота.
    /// Используется после команды /start.
    /// </summary>
    public static ReplyKeyboardMarkup MainMenu =>
        new(
            new[]
            {
                new KeyboardButton[]
                {
                    BotCommands.DrawCard
                }
            })
        {
            ResizeKeyboard = true,
            OneTimeKeyboard = false
        };
}