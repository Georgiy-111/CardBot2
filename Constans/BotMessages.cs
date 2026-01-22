namespace CardBot2.Constants;

/// <summary>
/// Тексты сообщений, отправляемых пользователю.
/// Вынесены отдельно для удобства поддержки и изменения.
/// </summary>
public static class BotMessages
{
    public const string StartMessage =
        "Привет! Нажми кнопку, чтобы вытянуть карту 🃏";

    public const string CardNotFound =
        "Файл карты не найден 😔";
    
    public const string UnknownCommand =
        "Я тебя не понял 🙂 Нажми кнопку 🃏 Вытянуть карту.";
}