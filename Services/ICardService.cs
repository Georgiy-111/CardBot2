using CardBot2.Domain;

namespace CardBot2.Services;

public interface ICardService
{
    Card GetRandomCard();
}