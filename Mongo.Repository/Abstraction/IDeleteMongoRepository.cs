using Mongo.Repository;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Mongo.Respository.Abstraction
{
    public interface IDeleteMongoRepository
    {
        #region Delete
               
        /// <summary>
        /// Asynchronously deletes a document matching the condition of the LINQ expression filter.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="filter">A LINQ expression filter.</param>
        /// <param name="partitionKey">An optional partition key.</param>
        /// <returns>The number of documents deleted.</returns>
        Task<bool> DeleteOneAsync<TDocument>(Expression<Func<TDocument, bool>> filter, string deletedBy, string partitionKey = null) where TDocument : IBaseModel;

        /// <summary>
        /// Deletes a document matching the condition of the LINQ expression filter.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="filter">A LINQ expression filter.</param>
        /// <param name="partitionKey">An optional partition key.</param>
        /// <returns>The number of documents deleted.</returns>
        bool DeleteOne<TDocument>(Expression<Func<TDocument, bool>> filter, string deletedBy, string partitionKey = null) where TDocument : IBaseModel;

        /// <summary>
        /// Asynchronously deletes the documents matching the condition of the LINQ expression filter.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="filter">A LINQ expression filter.</param>
        /// <param name="partitionKey">An optional partition key.</param>
        /// <returns>The number of documents deleted.</returns>
        Task<long> DeleteManyAsync<TDocument>(Expression<Func<TDocument, bool>> filter, string deletedBy, string partitionKey = null) where TDocument : IBaseModel;

        /// <summary>
        /// Deletes the documents matching the condition of the LINQ expression filter.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="filter">A LINQ expression filter.</param>
        /// <param name="partitionKey">An optional partition key.</param>
        /// <returns>The number of documents deleted.</returns>
        long DeleteMany<TDocument>(Expression<Func<TDocument, bool>> filter, string deletedBy, string partitionKey = null) where TDocument : IBaseModel;

        #endregion
    }
}
