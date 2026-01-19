using CardBot2.Context;
using CardBot2.Handlers.Commands;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;


namespace CardBot2.Handlers;

public class BotHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly List<ICommandHandler> _commandHandlers;

    public BotHandler(ITelegramBotClient botClient, List<ICommandHandler> commandHandlers)
    {
        _botClient = botClient;
        _commandHandlers = commandHandlers;
    }

    public async Task HandleUpdatesAsync()
    {
        int offset = 0;

        while (true)
        {
            var updates = await _botClient.GetUpdatesAsync(offset);

            foreach (var update in updates)
            {
                offset = update.Id + 1;

                if (update.Type != UpdateType.Message)
                    continue;

                var message = update.Message;
                if (message?.Text == null)
                    continue;

                var context = new BotContext(message);

                var handler = _commandHandlers.FirstOrDefault(h => h.Command == context.MessageText);
                if (handler != null)
                {
                    await handler.HandleAsync(context);
                }
            }

            await Task.Delay(500);
        }
    }
}