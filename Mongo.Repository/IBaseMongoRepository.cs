using Mongo.Respository.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Mongo.Repository
{

    /// <summary>
    /// The IBaseMongoRepository exposes the CRUD functionality of the BaseMongoRepository.
    /// </summary>
    public interface IBaseMongoRepository : 
        IReadOnlyMongoRepository, 
        ICreateMongoRepository, 
        IDeleteMongoRepository,
        IGroupingMongoRepository, 
        IProjectMongoRepository, 
        IUpdateMongoRepository, 
        IUpsertMongoRepository
    {        
        /// <summary>
        /// Asynchronously returns a paginated list of the documents matching the filter condition.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="filter"></param>
        /// <param name="skipNumber">The number of documents you want to skip. Default value is 0.</param>
        /// <param name="takeNumber">The number of documents you want to take. Default value is 50.</param>
        /// <param name="partitionKey">An optional partition key.</param>
        Task<List<TDocument>> GetPaginatedAsync<TDocument>(Expression<Func<TDocument, bool>> filter, int skipNumber = 0, int takeNumber = 50, string partitionKey = null)
            where TDocument:  IBaseModel;
    }

}