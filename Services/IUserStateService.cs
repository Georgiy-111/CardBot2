using CardBot2.Domain;

namespace CardBot2.Services;

/// <summary>
/// Контракт сервиса хранения состояния пользователя.
/// </summary>
public interface IUserStateService
{
    /// <summary>
    /// Получить текущее состояние пользователя.
    /// </summary>
    UserState GetState(long chatId);

    /// <summary>
    /// Установить новое состояние пользователя.
    /// </summary>
    void SetState(long chatId, UserState state);
}