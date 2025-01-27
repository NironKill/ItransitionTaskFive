using Bogus;
using BookStoreTesting.Application.Enums;
using BookStoreTesting.Application.Services.Interfaces;
using BookStoreTesting.Domain;
using System.Text;

namespace BookStoreTesting.Application.Services.Implementations
{
    public class BookService : IBookService
    {
        public IEnumerable<Book> GenerateBooks(string lang, int seed, double likes, double reviews, int numberOfBooks, int lastRowId)
        {
            Randomizer randomizer = new Randomizer(seed);

            Faker<Author> authorFarer = new Faker<Author>(lang)
                .UseSeed(seed)
                .RuleFor(a => a.FirstName, f => f.Name.FirstName())
                .RuleFor(a => a.LastName, f => f.Name.LastName());

            Faker<Review> reviewFaker = new Faker<Review>(lang)
                .UseSeed(seed)
                .RuleFor(a => a.Content, f => f.Lorem.Sentences())
                .RuleFor(a => a.CompanyName, f => f.Company.CompanyName())
                .RuleFor(a => a.Reviewer, f => f.Person.FullName);

            Faker<Book> bookFaker = new Faker<Book>(lang)
                .UseSeed(seed)
                .RuleFor(b => b.Title, f => f.Lorem.Sentence(3))
                .RuleFor(b => b.Publisher, f => f.Company.CompanyName())
                .RuleFor(b => b.Authors, f => authorFarer.Generate(f.Random.Int(1, 2)))
                .RuleFor(b => b.Reviews, reviewFaker.Generate(RandomWithFractions(randomizer, reviews)))
                .RuleFor(b => b.Likes, f => RandomWithFractions(randomizer, likes))
                .RuleFor(b => b.ImageUrl, f => f.Image.PicsumUrl());

            IEnumerable<Book> books = bookFaker.Generate(numberOfBooks).Select((book, index) =>
            {
                book.Id = lastRowId + index + 1;
                book.ISBN = GenerateISBN(randomizer, lang);
                return book;
            });
            return books;
        }
        private int RandomWithFractions(Randomizer randomizer, double maxCount)
        {
            int wholePart = (int)Math.Floor(maxCount);
            double fractionalPart = maxCount - wholePart;
            if (fractionalPart == 0)
                return wholePart;

            int result = wholePart;
            if (randomizer.Double() < fractionalPart)
                result++;

            return result;
        }
        private string GenerateISBN(Randomizer randomizer, string lang)
        {
            Language language = (Language)Enum.Parse(typeof(Language), lang);

            string publisher = randomizer.Number(10, 9999).ToString();
            
            string stringBuild = new StringBuilder("978" + language.GetHashCode().ToString() + publisher).ToString();

            string group = stringBuild.Length == 7 ? randomizer.UInt(10000, 99999).ToString()
                : stringBuild.Length == 8 ? randomizer.UInt(1000, 9999).ToString()
                : stringBuild.Length == 9 ? randomizer.UInt(100, 999).ToString()
                : randomizer.UInt(100000, 999999).ToString();

            stringBuild += group;

            int remainder = default;
            for (int i = 0; i < stringBuild.Length; i++)
            {
                if (i % 2 == 0)
                    remainder += int.Parse(stringBuild[i].ToString());
                else
                    remainder += int.Parse(stringBuild[i].ToString()) * 3;
            }

            remainder %= 10;

            int сheckDigit = remainder > 0 ? 10 - remainder : 0;

            string isbn = $"978-{language.GetHashCode()}-{publisher}-{group}-{сheckDigit}";

            return isbn;
        }
    }
}
