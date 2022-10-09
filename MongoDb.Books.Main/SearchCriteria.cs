using System.Runtime.Serialization;

namespace MongoDb.Books.Main
{
    /// <summary>
    ///     Represents the search criteria
    /// </summary>
    [DataContract]
    public class SearchCriteria
    {
        /// <summary>
        ///     The limit
        /// </summary>
        [DataMember]
        public int Limit { get; set; }

        /// <summary>
        ///     The offset
        /// </summary>
        [DataMember]
        public int Offset { get; set; }

        /// <summary>
        ///     The book isbn
        /// </summary>
        [DataMember]
        public string Isbn { get; set; }

        /// <summary>
        ///     The title
        /// </summary>
        [DataMember]
        public string? Title { get; set; }

        /// <summary>
        ///     A list of authors
        /// </summary>
        [DataMember]
        public IList<string> Authors { get; set; } = new List<string>();
    }
}
