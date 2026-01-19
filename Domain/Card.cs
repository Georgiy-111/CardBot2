namespace CardBot2.Domain;

/// <summary>
/// Доменная модель карты.
/// Описывает одну карту, которая может быть выдана пользователю ботом.
/// 
/// ВАЖНО:
/// - Этот класс не содержит логики
/// - Не знает о Telegram
/// - Не знает о файлах или JSON
/// Используется как чистая модель данных (POCO).
/// </summary>
public class Card
{
    /// <summary>
    /// Отображаемое название карты.
    /// Показывается пользователю в Telegram.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Текстовое описание карты.
    /// Используется как подпись под изображением.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Относительный путь к изображению карты в файловой системе.
    /// Пример: Assets/cards/Card1.png
    /// </summary>
    public string ImagePath { get; set; } = string.Empty;
}