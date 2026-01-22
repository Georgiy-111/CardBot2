using CardBot2.Domain;

namespace CardBot2.Services;

/// <summary>
/// In-memory реализация хранения состояний пользователей.
/// 
/// Ключ: ChatId
/// Значение: UserState
/// 
/// При перезапуске бота состояния сбрасываются — это нормально
/// для текущего этапа развития.
/// </summary>
public class UserStateService : IUserStateService
{
    private readonly Dictionary<long, UserState> _states = new();

    public UserState GetState(long chatId)
    {
        return _states.TryGetValue(chatId, out var state)
            ? state
            : UserState.None;
    }

    public void SetState(long chatId, UserState state)
    {
        _states[chatId] = state;
    }
}