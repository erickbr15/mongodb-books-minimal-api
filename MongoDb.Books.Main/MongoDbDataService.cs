using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDb.Books.Main
{
    /// <summary>
    ///     Concrete implementation of <see cref="IMongoDbDataService"/>
    /// </summary>
    public class MongoDbDataService : IMongoDbDataService
    {
        private static string _ConnectionString = "mongodb://localhost:27017";
        private static string _DatabaseName = "libraryDb";
        private static string _Collection = "books";

        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;

        /// <summary>
        ///     Creates an instance of <see cref="MongoDbDataService"/>
        /// </summary>
        public MongoDbDataService()
        {
            _client = new MongoClient(_ConnectionString);
            _database = _client.GetDatabase(_DatabaseName);
        }

        /// <inheritdoc />
        public async Task<bool> Exists(ObjectId bookId, CancellationToken cancellationToken)
        {
            var books = _database.GetCollection<Book>(_Collection);
            var booksCursor = await books.FindAsync(b => b._id == bookId, options: null, cancellationToken);

            var exists = await booksCursor.AnyAsync(cancellationToken);

            return exists;
        }

        /// <inheritdoc />
        public IEnumerable<Book> Get(int limit = 100, int offset = 0)
        {
            var booksCollection = _database.GetCollection<Book>(_Collection);

            var books = booksCollection.AsQueryable().Skip(offset).Take(limit).ToList();

            return books;
        }

        /// <inheritdoc />
        public async Task<Book> GetByIdAsync(ObjectId bookId, CancellationToken cancellationToken)
        {
            var books = _database.GetCollection<Book>(_Collection);

            var booksCursor = await books.FindAsync(b => b._id == bookId, options: null, cancellationToken);
            var book = await booksCursor.SingleOrDefaultAsync(cancellationToken);

            return book;
        }

        /// <inheritdoc />
        public async Task<Book?> CreateAsync(Book book, CancellationToken cancellationToken)
        {
            if (book is null)
            {
                return null;
            }

            var books = _database.GetCollection<Book>(_Collection);

            await books.InsertOneAsync(book);

            return book;
        }

        /// <inheritdoc />
        public async Task<Book?> UpdateAsync(ObjectId bookId, Book book, CancellationToken cancellationToken)
        {
            var existsBook = await Exists(bookId, cancellationToken);
            if (!existsBook)
            {
                return null;
            }

            var books = _database.GetCollection<Book>(_Collection);

            var replacedBook = await books.FindOneAndReplaceAsync(b => b._id == bookId, book, options: null, cancellationToken);

            return replacedBook;
        }

        /// <inheritdoc />
        public async Task DeleteAsync(ObjectId bookId, CancellationToken cancellationToken)
        {
            var existsBook = await Exists(bookId, cancellationToken);
            if (!existsBook)
            {
                return;
            }

            var books = _database.GetCollection<Book>(_Collection);

            await books.FindOneAndDeleteAsync(b => b._id == bookId, options: null, cancellationToken);
        }
    }
}
