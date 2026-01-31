namespace CardBot2.Constants;

/// <summary>
/// Текстовые сообщения Telegram-бота.
/// </summary>
public static class BotMessages
{
    public const string StartMessage =
        "Привет! 👋\n" +
        "Я бот с метафорическими картами.\n\n" +
        "Ты можешь:\n" +
        "🃏 вытянуть случайную карту\n" +
        "ℹ️ узнать, как работает бот";

    public const string HelpMessage =
        "🃏 *Как работает бот*\n\n" +
        "Ты нажимаешь кнопку «Вытянуть карту»,\n" +
        "а я случайным образом выбираю карту\n" +
        "и отправляю тебе её изображение с описанием.\n\n" +
        "Используй карты для размышлений,\n" +
        "самоанализа или вдохновения.";

    public const string UnknownCommand =
        "Я тебя не понял 🙂\nНажми кнопку 🃏 Вытянуть карту.";

    public const string CardNotFound =
        "Файл карты не найден 😔";
}