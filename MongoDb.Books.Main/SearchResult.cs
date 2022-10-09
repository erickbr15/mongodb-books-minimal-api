using System.Runtime.Serialization;

namespace MongoDb.Books.Main
{
    /// <summary>
    ///     The search results
    /// </summary>
    [DataContract]
    public class SearchResult
    {
        /// <summary>
        ///     The criteria matching books
        /// </summary>
        [DataMember]
        public IList<Book> Books { get; set; } = new List<Book>();

        /// <summary>
        ///     The total of books
        /// </summary>
        [DataMember]
        public int TotalBooks { get; set; }
    }
}
