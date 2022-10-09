using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;

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

        /// <inheritdoc />
        public async Task<SearchResult?> SearchAsync(SearchCriteria criteria, CancellationToken cancellationToken)
        {
            if (criteria == null)
            {
                return new SearchResult();
            }
            
            var searchResult = new SearchResult();

            if (!string.IsNullOrWhiteSpace(criteria.Isbn))
            {
                searchResult = await SearchByIsbnAsync(criteria.Isbn, criteria.Offset, criteria.Limit, cancellationToken);
                return searchResult;
            }

            var shouldSearchByTitle = !string.IsNullOrWhiteSpace(criteria.Title);
            var shouldSearchByAuthors = criteria.Authors != null && criteria.Authors.Any();            

            if (shouldSearchByTitle && shouldSearchByAuthors)
            {
                searchResult = await SearchByTitleAndAuthors(criteria.Title, criteria.Authors, criteria.Offset, criteria.Limit, cancellationToken);
            }
            else if (shouldSearchByTitle)
            {
                searchResult = await SearchByTitle(criteria.Title, criteria.Offset, criteria.Limit, cancellationToken);
            }
            else if (shouldSearchByAuthors)
            {
                searchResult = await SearchByAuthors(criteria.Authors, criteria.Offset, criteria.Limit, cancellationToken);
            }
            
            return searchResult;
        }

        /// <summary>
        ///     Searches a book by ISBN
        /// </summary>
        /// <param name="isbn">
        ///     The ISBN
        /// </param>
        /// <param name="offset">
        ///     The offset
        /// </param>
        /// <param name="limit">
        ///     The limit
        /// </param>
        /// <param name="cancellationToken">
        ///     The operation cancellation token. An instance of <see cref="CancellationToken"/>
        /// </param>
        /// <returns>
        ///     The search results. An instance of <see cref="SearchResult"/>
        /// </returns>
        private async Task<SearchResult> SearchByIsbnAsync(string isbn, int offset, int limit, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(isbn))
            {
                return new SearchResult();
            }
            
            var booksCollection = _database.GetCollection<Book>(_Collection);
            var booksCursor = booksCollection.Find(b => b.Isbn == isbn);

            var searchResult = new SearchResult
            {
                TotalBooks = (int)await booksCursor.CountDocumentsAsync(cancellationToken),
                Books = await booksCursor.Skip(offset * limit).Limit(limit).ToListAsync(cancellationToken)
            };

            return searchResult;
        }

        /// <summary>
        ///     Searches a book by title and a list of authors
        /// </summary>
        /// <param name="title">
        ///     The title
        /// </param>
        /// <param name="authors">
        ///     The list of authors
        /// </param>
        /// <param name="offset">
        ///     The offset
        /// </param>
        /// <param name="limit">
        ///     The limit
        /// </param>
        /// <param name="cancellationToken">
        ///     The operation cancellation token. An instance of <see cref="CancellationToken"/>
        /// </param>
        /// <returns>
        ///     The search results. An instance of <see cref="SearchResult"/>
        /// </returns>
        private async Task<SearchResult> SearchByTitleAndAuthors(string title, IEnumerable<string> authors, int offset, int limit, CancellationToken cancellationToken)
        {                        
            var booksCollection = _database.GetCollection<Book>(_Collection);

            var booksCursor = booksCollection.Find(b => b.Title != null && b.Title.Contains(title)
                && b.Authors.Any(x => authors.Contains(x)));

            var searchResult = new SearchResult
            {
                TotalBooks = (int)await booksCursor.CountDocumentsAsync(cancellationToken),
                Books = await booksCursor.Skip(offset * limit).Limit(limit).ToListAsync(cancellationToken)
            };

            return searchResult;
        }

        /// <summary>
        ///     Searches a book by title
        /// </summary>
        /// <param name="title">
        ///     The title
        /// </param>
        /// <param name="offset">
        ///     The offset
        /// </param>
        /// <param name="limit">
        ///     The limit
        /// </param>
        /// <param name="cancellationToken">
        ///     The operation cancellation token. An instance of <see cref="CancellationToken"/>
        /// </param>
        /// <returns>
        ///     The search results. An instance of <see cref="SearchResult"/>
        /// </returns>
        private async Task<SearchResult> SearchByTitle(string title, int offset, int limit, CancellationToken cancellationToken)
        {
            var booksCollection = _database.GetCollection<Book>(_Collection);
            var booksCursor = booksCollection.Find(b => b.Title != null && b.Title.Contains(title));

            var searchResult = new SearchResult
            {
                TotalBooks = (int)await booksCursor.CountDocumentsAsync(cancellationToken),
                Books = await booksCursor.Skip(offset * limit).Limit(limit).ToListAsync(cancellationToken)
            };

            return searchResult;
        }

        /// <summary>
        ///     Searches a book by a list of authors
        /// </summary>
        /// <param name="authors">
        ///     The list of authors
        /// </param>
        /// <param name="offset">
        ///     The offset
        /// </param>
        /// <param name="limit">
        ///     The limit
        /// </param>
        /// <param name="cancellationToken">
        ///     The operation cancellation token. An instance of <see cref="CancellationToken"/>
        /// </param>
        /// <returns>
        ///     The search results. An instance of <see cref="SearchResult"/>
        /// </returns>
        private async Task<SearchResult> SearchByAuthors(IEnumerable<string> authors, int offset, int limit, CancellationToken cancellationToken)
        {
            var booksCollection = _database.GetCollection<Book>(_Collection);
            var booksCursor = booksCollection.Find(b => b.Authors.Any(x => authors.Contains(x)));

            var searchResult = new SearchResult
            {
                TotalBooks = (int)await booksCursor.CountDocumentsAsync(cancellationToken),
                Books = await booksCursor.Skip(offset * limit).Limit(limit).ToListAsync(cancellationToken)
            };

            return searchResult;
        }
    }
}
