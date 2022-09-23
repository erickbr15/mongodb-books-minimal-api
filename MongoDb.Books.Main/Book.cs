using MongoDB.Bson;
using System.Runtime.Serialization;

namespace MongoDb.Books.Main
{
    /// <summary>
    ///     Represents a book
    /// </summary>
    [DataContract]
    public class Book
    {
        /// <summary>
        ///     The book id
        /// </summary>
        [DataMember]
        public ObjectId _id { get; set; }

        /// <summary>
        ///     The book title
        /// </summary>
        [DataMember]
        public string? Title { get; set; }

        /// <summary>
        ///     The authors
        /// </summary>
        [DataMember]
        public IList<string> Authors { get; set; } = new List<string>();

        /// <summary>
        ///     The average rating
        /// </summary>
        [DataMember]
        public decimal AverageRating { get; set; }

        /// <summary>
        ///     The ISBN
        /// </summary>
        [DataMember]
        public string? Isbn { get; set; }

        /// <summary>
        ///     The ISBN13
        /// </summary>
        [DataMember]
        public string? Isbn13 { get; set; }

        /// <summary>
        ///     The language code
        /// </summary>
        [DataMember]
        public string? LanguageCode { get; set; }

        /// <summary>
        ///     The number of pages
        /// </summary>
        [DataMember]
        public int? NumPages { get; set; }

        /// <summary>
        ///     The ratings count
        /// </summary>
        [DataMember]
        public int RatingsCount { get; set; }

        /// <summary>
        ///     The text reviews count
        /// </summary>
        [DataMember]
        public int TextReviewsCount { get; set; }

        /// <summary>
        ///     The publication date
        /// </summary>
        [DataMember]
        public DateTime? PublicationDate { get; set; }

        /// <summary>
        ///     The publisher
        /// </summary>
        [DataMember]
        public string? Publisher { get; set; }
    }
}