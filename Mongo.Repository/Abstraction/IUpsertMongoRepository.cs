using Common.Mongo.Repository;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace Common.Mongo.Respository.Abstraction
{
    public interface IUpsertMongoRepository
    {
        #region upsert

        /// <summary>
        /// GetAndUpdateOne with filter
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="filter"></param>
        /// <param name="update"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        Task<TDocument> GetAndUpdateOne<TDocument>(FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, FindOneAndUpdateOptions<TDocument, TDocument> options, string updatedBy)
            where TDocument : IBaseModel;

        #endregion
    }
}
