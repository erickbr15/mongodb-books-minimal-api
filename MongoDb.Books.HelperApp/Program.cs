using Microsoft.Extensions.DependencyInjection;
using MongoDb.Books.Main;
using System.IO;

namespace MongoDb.Books.HelperApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var books = ReadFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "books.data.csv"));
            var dataService = new MongoDbDataService();

            foreach (var book in books)
            {
                dataService.CreateAsync(book, cancellationToken: default).Wait();
            }
        }

        private static IList<Book> ReadFile(string fullpath)
        {
            var books = new List<Book>();
            var lines = File.ReadAllLines(fullpath);

            for (int i = 1; i < 2000; i++)
            {
                var fields = lines[i].Split(",", StringSplitOptions.RemoveEmptyEntries);
                var book = BuildBook(fields);
                if (book != null)
                {
                    books.Add(book);
                }
            }

            return books;
        }

        private static Book? BuildBook(string[] fields)
        {
            const int EXPECTED_FIELDS = 12;

            if (fields.Length != EXPECTED_FIELDS)
            {
                return null;
            }

            var book = new Book
            {
                Title = fields[Indexes.Title],
                Authors = new List<string>((fields[Indexes.Authors] ?? string.Empty).Split("/", StringSplitOptions.RemoveEmptyEntries)),
                AverageRating = decimal.Parse(fields[Indexes.AverageRating]),
                Isbn = fields[Indexes.Isbn],
                Isbn13 = fields[Indexes.Isbn13],
                LanguageCode = fields[Indexes.LanguageCode],
                NumPages = int.Parse(fields[Indexes.NumPages]),
                RatingsCount = int.Parse(fields[Indexes.RatingsCount]),
                TextReviewsCount = int.Parse(fields[Indexes.TextReviews]),
                PublicationDate = GetPublicationDate(fields[Indexes.PublicationDate]),
                Publisher = fields[Indexes.Publisher]
            };

            return book;
        }

        private static DateTime? GetPublicationDate(string field)
        {            
            try
            {
                var dateParts = field.Split("/", StringSplitOptions.RemoveEmptyEntries);
                return new DateTime(int.Parse(dateParts[2]), int.Parse(dateParts[1]), int.Parse(dateParts[0]));
            }
            catch
            {
                return null;
            }
            
        }


        /// <summary>
        ///     Indexes
        /// </summary>
        private static class Indexes
        {
            public static int Title = 1;
            public static int Authors = 2;
            public static int AverageRating = 3;
            public static int Isbn = 4;
            public static int Isbn13 = 5;
            public static int LanguageCode = 6;
            public static int NumPages = 7;
            public static int RatingsCount = 8;
            public static int TextReviews = 9;
            public static int PublicationDate = 10;
            public static int Publisher = 11;            
        }
    }

    
}