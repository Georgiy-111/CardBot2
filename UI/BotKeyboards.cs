using CardBot2.Constants;
using Telegram.Bot.Types.ReplyMarkups;

namespace CardBot2.UI;

/// <summary>
/// Фабрика клавиатур для Telegram-бота.
/// 
/// Отвечает только за:
/// - создание клавиатур (ReplyKeyboardMarkup или InlineKeyboardMarkup)
/// - конфигурацию кнопок
/// 
/// Не содержит никакой бизнес-логики.
/// </summary>
public static class BotKeyboards
{
    /// <summary>
    /// Главная клавиатура бота.
    /// 
    /// Используется после команды /start и в других сообщениях,
    /// чтобы пользователь всегда видел кнопку для вытягивания карты.
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
            // Автоматически подгоняет размер кнопок под экран
            ResizeKeyboard = true,
            
            // Клавиатура остаётся на экране после нажатия кнопки
            OneTimeKeyboard = false
        };
}