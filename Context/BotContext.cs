using Telegram.Bot.Types;

namespace CardBot2.Context;

/// <summary>
/// Контекст одного входящего сообщения Telegram.
/// Используется для передачи данных между слоями.
/// </summary>
public class BotContext
{
    public long ChatId { get; }
    public string MessageText { get; }
    public Message RawMessage { get; }

    public BotContext(Message message)
    {
        ChatId = message.Chat.Id;
        MessageText = message.Text!;
        RawMessage = message;
    }
}