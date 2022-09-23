using MongoDB.Bson;

namespace MongoDb.Books.Main
{
    /// <summary>
    ///     MongoDB data service contract
    /// </summary>
    public interface IMongoDbDataService
    {
        /// <summary>
        ///     Verifies if a books exists given the bookId
        /// </summary>
        /// <param name="bookId">
        ///     The book id
        /// </param>
        /// <param name="cancellationToken">
        ///     The operation cancellation token. An instance of <see cref="CancellationToken"/>
        /// </param>
        /// <returns>
        ///     true if the book exists, otherwise false
        /// </returns>
        Task<bool> Exists(ObjectId bookId, CancellationToken cancellationToken);

        /// <summary>
        ///     Gets a set of books
        /// </summary>
        /// <param name="limit">
        ///     The limit of books. 100 by default.
        /// </param>
        /// <param name="offset">
        ///     The offset. 0 by default.
        /// </param>
        /// <returns>
        ///     A list of books.
        /// </returns>
        IEnumerable<Book> Get(int limit = 100, int offset = 0);

        /// <summary>
        ///     Gets a book by id
        /// </summary>
        /// <param name="bookId">
        ///     The book id
        /// </param>
        /// <param name="cancellationToken">
        ///     The operation cancellation token. An instance of <see cref="CancellationToken"/>
        /// </param>
        /// <returns>
        ///     If the book exists, an instance of <see cref="Book"/>, otherwise null
        /// </returns>
        Task<Book> GetByIdAsync(ObjectId bookId, CancellationToken cancellationToken);

        /// <summary>
        ///     Creates a new book
        /// </summary>
        /// <param name="book">
        ///     The book to create
        /// </param>
        /// <param name="cancellationToken">
        ///     The operation cancellation token. An instance of <see cref="CancellationToken"/>
        /// </param>
        /// <returns>
        ///     An instance of <see cref="Book"/> which represents the created book
        /// </returns>
        Task<Book?> CreateAsync(Book book, CancellationToken cancellationToken);

        /// <summary>
        ///     Updates an existing book
        /// </summary>
        /// <param name="bookId">
        ///     The book id
        /// </param>
        /// <param name="book">
        ///     The book to update
        /// </param>
        /// <param name="cancellationToken">
        ///     The operation cancellation token. An instance of <see cref="CancellationToken"/>
        /// </param>
        /// <returns>
        ///     An instance of <see cref="Book"/> which represent the updated book
        /// </returns>
        Task<Book?> UpdateAsync(ObjectId bookId, Book book, CancellationToken cancellationToken);

        /// <summary>
        ///     Deletes a book from the database
        /// </summary>
        /// <param name="bookId">
        ///     The book id
        /// </param>
        /// <param name="cancellationToken">
        ///     The operation cancellation token. An instance of <see cref="CancellationToken"/>
        /// </param>
        Task DeleteAsync(ObjectId bookId, CancellationToken cancellationToken);
    }
}
