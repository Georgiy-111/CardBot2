using CardBot2.Domain;

namespace CardBot2.Services;

public sealed class CardService
{
    private readonly List<Card> _cards =
    [
        new Card
        {
            Name = "Карта 1",
            Description = "Описание первой карты",
            ImagePath = "Assets/cards/Card1.png"
        },
        new Card
        {
            Name = "Карта 2",
            Description = "Описание второй карты",
            ImagePath = "Assets/cards/Card2.png"
        },
        new Card
        {
            Name = "Карта 3",
            Description = "Описание третьей карты",
            ImagePath = "Assets/cards/Card3.png"
        }
    ];

    public Card GetRandomCard()
    {
        var index = Random.Shared.Next(_cards.Count);
        return _cards[index];
    }
}