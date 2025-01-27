using BookStoreTesting.Domain;

namespace BookStoreTesting.Application.Services.Interfaces
{
    public interface IBookService
    {
        IEnumerable<Book> GenerateBooks(string language, int seed, double likes, double reviews, int numberOfBooks, int lastRowId);
    }
}
