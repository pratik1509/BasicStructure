using Common.Mongo.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Mongo.Respository.Abstraction
{
    public interface ICreateMongoRepository
    {
        #region Create

        /// <summary>
        /// Asynchronously adds a document to the collection.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="document">The document you want to add.</param>
        Task<string> AddOneAsync<TDocument>(TDocument document, string createdBy) where TDocument : IBaseModel;

        /// <summary>
        /// Adds a document to the collection.
        /// Populates the Id and AddedAtUtc fields if necessary.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="document">The document you want to add.</param>
        string AddOne<TDocument>(TDocument document, string createdBy) where TDocument : IBaseModel;

        /// <summary>
        /// Asynchronously adds a list of documents to the collection.
        /// Populates the Id and AddedAtUtc fields if necessary.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="documents">The document you want to add.</param>
        Task AddManyAsync<TDocument>(IEnumerable<TDocument> documents, string createdBy) where TDocument : IBaseModel;

        /// <summary>
        /// Adds a list of documents to the collection.
        /// Populates the Id and AddedAtUtc fields if necessary.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="documents">The document you want to add.</param>
        void AddMany<TDocument>(IEnumerable<TDocument> documents, string createdBy) where TDocument : IBaseModel;

        #endregion
    }
}
