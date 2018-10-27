using Common.Mongo.Respository.Abstraction;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Common.Mongo.Repository
{
    /// <summary>
    /// The ReadOnlyMongoRepository implements the readonly functionality of the IReadOnlyMongoRepository.
    /// </summary>
    public class ReadOnlyMongoRepository : IReadOnlyMongoRepository
    {
        /// <summary>
        /// The connection string.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// The database name.
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        /// The MongoDbContext
        /// </summary>
        protected IMongoDbContext MongoDbContext = null;

        /// <summary>
        /// The constructor taking a connection string and a database name.
        /// </summary>
        /// <param name="connectionString">The connection string of the MongoDb server.</param>
        /// <param name="databaseName">The name of the database against which you want to perform operations.</param>
        protected ReadOnlyMongoRepository(string connectionString, string databaseName)
        {
            MongoDbContext = new MongoDbContext(connectionString, databaseName);
        }

        /// <summary>
        /// The contructor taking a <see cref="IMongoDbContext"/>.
        /// </summary>
        /// <param name="mongoDbContext">A mongodb context implementing <see cref="IMongoDbContext"/></param>
        protected ReadOnlyMongoRepository(IMongoDbContext mongoDbContext)
        {
            MongoDbContext = mongoDbContext;
        }

        /// <summary>
        /// The contructor taking a <see cref="IMongoDatabase"/>.
        /// </summary>
        /// <param name="mongoDatabase">A mongodb context implementing <see cref="IMongoDatabase"/></param>
        protected ReadOnlyMongoRepository(IMongoDatabase mongoDatabase)
        {
            MongoDbContext = new MongoDbContext(mongoDatabase);
        }

        #region Read

        /// <summary>
        /// Asynchronously returns one document given its id.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="id">The Id of the document you want to get.</param>
        /// <param name="partitionKey">An optional partition key.</param>
        public async Task<TDocument> GetByIdAsync<TDocument>(Guid id, string partitionKey = null) where TDocument : IBaseModel
        {
            var filter = Builders<TDocument>.Filter.Eq("Id", id);
            return await GetCollection<TDocument>().Find(filter).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Returns one document given its id.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="id">The Id of the document you want to get.</param>
        /// <param name="partitionKey">An optional partition key.</param>
        public TDocument GetById<TDocument>(Guid id, string partitionKey = null) where TDocument : IBaseModel
        {
            var filter = Builders<TDocument>.Filter.Eq("Id", id);
            return GetCollection<TDocument>().Find(filter).FirstOrDefault();
        }

        /// <summary>
        /// Asynchronously returns one document given an expression filter if its not deleted.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="filter">A LINQ expression filter.</param>
        /// <param name="partitionKey">An optional partition key.</param>
        public async Task<TDocument> GetOneAsync<TDocument>(Expression<Func<TDocument, bool>> filter, string partitionKey = null) where TDocument : IBaseModel
        {
            var compositeFilter = new FilterDefinitionBuilder<TDocument>();
            var videoFilterDef = compositeFilter.Empty;
            videoFilterDef = filter & compositeFilter.Eq(x => x.IsDeleted, false);

            return await GetCollection<TDocument>().Find(videoFilterDef).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Asynchronously returns one document given an expression filter even though its value is set as deleted.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="filter">A LINQ expression filter.</param>
        /// <param name="partitionKey">An optional partition key.</param>
        public async Task<TDocument> GetOneAsyncWithDeleted<TDocument>(Expression<Func<TDocument, bool>> filter, string partitionKey = null) where TDocument : IBaseModel
        {
            return await GetCollection<TDocument>().Find(filter).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Returns one document given an expression filter if it is not deleted.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="filter">A LINQ expression filter.</param>
        /// <param name="partitionKey">An optional partition key.</param>
        public TDocument GetOne<TDocument>(Expression<Func<TDocument, bool>> filter, string partitionKey = null) where TDocument : IBaseModel
        {
            var compositeFilter = new FilterDefinitionBuilder<TDocument>();
            var videoFilterDef = compositeFilter.Empty;
            videoFilterDef = filter & compositeFilter.Eq(x => x.IsDeleted, false);

            return GetCollection<TDocument>().Find(videoFilterDef).FirstOrDefault();
        }

        /// <summary>
        /// Returns one document given an expression filter even though it is deleted.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="filter">A LINQ expression filter.</param>
        /// <param name="partitionKey">An optional partition key.</param>
        public TDocument GetOneWithDeleted<TDocument>(Expression<Func<TDocument, bool>> filter, string partitionKey = null) where TDocument : IBaseModel
        {
            return GetCollection<TDocument>().Find(filter).FirstOrDefault();
        }

        /// <summary>
        /// Returns a collection cursor.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="filter">A LINQ expression filter.</param>
        /// <param name="partitionKey">An optional partition key.</param>
        public IFindFluent<TDocument, TDocument> GetCursor<TDocument>(Expression<Func<TDocument, bool>> filter, string partitionKey = null) where TDocument : IBaseModel
        {
            return GetCollection<TDocument>().Find(filter);
        }

        /// <summary>
        /// Returns true if any of the document of the collection matches the filter condition.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="filter">A LINQ expression filter.</param>
        /// <param name="partitionKey">An optional partition key.</param>
        public async Task<bool> AnyAsync<TDocument>(Expression<Func<TDocument, bool>> filter, string partitionKey = null) where TDocument : IBaseModel
        {
            var count = await GetCollection<TDocument>().CountDocumentsAsync(filter);
            return (count > 0);
        }

        /// <summary>
        /// Returns true if any of the document of the collection matches the filter condition.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="filter">A LINQ expression filter.</param>
        /// <param name="partitionKey">An optional partition key.</param>
        public bool Any<TDocument>(Expression<Func<TDocument, bool>> filter, string partitionKey = null) where TDocument : IBaseModel
        {
            var count = GetCollection<TDocument>().CountDocuments(filter);
            return (count > 0);
        }

        /// <summary>
        /// Asynchronously returns a list of the documents matching the filter condition if its not IsDeleted.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="filter">A LINQ expression filter.</param>
        /// <param name="partitionKey">An optional partition key.</param>
        public async Task<List<TDocument>> GetAllAsync<TDocument>(Expression<Func<TDocument, bool>> filter, string partitionKey = null) where TDocument : IBaseModel
        {
            var compositeFilter = new FilterDefinitionBuilder<TDocument>();
            var videoFilterDef = compositeFilter.Empty;
            videoFilterDef = filter & compositeFilter.Eq(x => x.IsDeleted, false);

            return await GetCollection<TDocument>().Find(videoFilterDef).ToListAsync();
        }

        /// <summary>
        /// Asynchronously returns a list of the documents matching the filter condition if its not IsDeleted.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="filter">A LINQ expression filter.</param>
        /// <param name="partitionKey">An optional partition key.</param>
        public async Task<List<TDocument>> FindAsync<TDocument>(FilterDefinition<TDocument> filter, string partitionKey = null) where TDocument : IBaseModel
        {
            var compositeFilter = new FilterDefinitionBuilder<TDocument>();
            var videoFilterDef = compositeFilter.Empty;
            videoFilterDef = filter & compositeFilter.Eq(x => x.IsDeleted, false);

            return await GetCollection<TDocument>().Find(videoFilterDef).ToListAsync();
        }

        /// <summary>
        /// Asynchronously returns a list of the documents matching the filter condition if its not IsDeleted.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="filter">A LINQ expression filter.</param>
        /// <param name="partitionKey">An optional partition key.</param>
        public async Task<List<TNewProjection>> FindAndProjectAsync<TDocument, TNewProjection>
            (FilterDefinition<TDocument> filter, Expression<Func<TDocument, TNewProjection>> projection, string partitionKey = null) where TDocument : IBaseModel
        {
            var compositeFilter = new FilterDefinitionBuilder<TDocument>();
            var videoFilterDef = compositeFilter.Empty;
            videoFilterDef = filter & compositeFilter.Eq(x => x.IsDeleted, false);

            return await GetCollection<TDocument>()
                .Find(videoFilterDef)
                .Project(projection)
                .ToListAsync();
        }

        /// <summary>
        /// Asynchronously returns a list of the documents matching the filter condition eventhough its deleted
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="filter">A LINQ expression filter.</param>
        /// <param name="partitionKey">An optional partition key.</param>
        public async Task<List<TDocument>> GetAllAsyncWithDeleted<TDocument>(Expression<Func<TDocument, bool>> filter, string partitionKey = null) where TDocument : IBaseModel
        {
            return await GetCollection<TDocument>().Find(filter).ToListAsync();
        }

        /// <summary>
        /// Returns a list of the documents matching the filter condition excluding IsDeleted records.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="filter">A LINQ expression filter.</param>
        /// <param name="partitionKey">An optional partition key.</param>
        public List<TDocument> GetAll<TDocument>(Expression<Func<TDocument, bool>> filter, string partitionKey = null) where TDocument : IBaseModel
        {
            var compositeFilter = new FilterDefinitionBuilder<TDocument>();
            var videoFilterDef = compositeFilter.Empty;
            videoFilterDef = filter & compositeFilter.Eq(x => x.IsDeleted, false);

            return GetCollection<TDocument>().Find(videoFilterDef).ToList();
        }

        /// <summary>
        /// Returns a list of the documents matching the filter condition excluding IsDeleted records.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="filter">A filter defination.</param>
        /// <param name="partitionKey">An optional partition key.</param>
        public List<TDocument> Find<TDocument>(FilterDefinition<TDocument> filter, string partitionKey = null) where TDocument : IBaseModel
        {
            var compositeFilter = new FilterDefinitionBuilder<TDocument>();
            var videoFilterDef = compositeFilter.Empty;
            videoFilterDef = filter & compositeFilter.Eq(x => x.IsDeleted, false);

            return GetCollection<TDocument>().Find(videoFilterDef).ToList();
        }

        /// <summary>
        /// returns a list of the documents matching the filter condition if its not IsDeleted.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="filter">A LINQ expression filter.</param>
        /// <param name="partitionKey">An optional partition key.</param>
        public List<TNewProjection> FindAndProject<TDocument, TNewProjection>
            (FilterDefinition<TDocument> filter, Expression<Func<TDocument, TNewProjection>> projection, string partitionKey = null) where TDocument : IBaseModel
        {
            var compositeFilter = new FilterDefinitionBuilder<TDocument>();
            var videoFilterDef = compositeFilter.Empty;
            videoFilterDef = filter & compositeFilter.Eq(x => x.IsDeleted, false);

            return GetCollection<TDocument>()
                .Find(videoFilterDef)
                .Project(projection)
                .ToList();
        }

        /// <summary>
        /// Returns a list of the documents matching the filter condition including IsDeleted records.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="filter">A LINQ expression filter.</param>
        /// <param name="partitionKey">An optional partition key.</param>
        public List<TDocument> GetAllWithIsDeleted<TDocument>(Expression<Func<TDocument, bool>> filter, string partitionKey = null) where TDocument : IBaseModel
        {
            return GetCollection<TDocument>().Find(filter).ToList();
        }

        /// <summary>
        /// Asynchronously counts how many documents match the filter condition.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="filter">A LINQ expression filter.</param>
        /// <param name="partitionKey">An optional partitionKey</param>
        public async Task<long> CountAsync<TDocument>(Expression<Func<TDocument, bool>> filter, string partitionKey = null) where TDocument : IBaseModel
        {
            return await GetCollection<TDocument>().CountDocumentsAsync(filter);
        }

        /// <summary>
        /// Counts how many documents match the filter condition.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="filter">A LINQ expression filter.</param>
        /// <param name="partitionKey">An optional partitionKey</param>
        public long Count<TDocument>(Expression<Func<TDocument, bool>> filter, string partitionKey = null) where TDocument : IBaseModel
        {
            return GetCollection<TDocument>().Find(filter).CountDocuments();
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Gets a collections for the type TDocument with the matching partition key (if any).
        /// </summary>
        /// <typeparam name="TDocument">The document type.</typeparam>
        /// <param name="partitionKey">An optional partition key.</param>
        /// <returns>An <see cref="IMongoCollection{TDocument}"/></returns>
        protected IMongoCollection<TDocument> GetCollection<TDocument>(string partitionKey = null) where TDocument : IBaseModel
        {
            return MongoDbContext.GetCollection<TDocument>(partitionKey);
        }

        /// <summary>
        /// Gets a collections for a potentially partitioned document type.
        /// </summary>
        /// <typeparam name="TDocument">The document type.</typeparam>
        /// <param name="partitionKey">The collection partition key.</param>
        /// <returns></returns>
        protected IMongoCollection<TDocument> HandlePartitioned<TDocument>(string partitionKey) where TDocument : IBaseModel
        {
            if (!string.IsNullOrEmpty(partitionKey))
            {
                return GetCollection<TDocument>(partitionKey);
            }
            return GetCollection<TDocument>();
        }

        /// <summary>
        /// Converts a LINQ expression of TDocument, TValue to a LINQ expression of TDocument, object
        /// </summary>
        /// <typeparam name="TDocument">The document type.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="expression">The expression to convert</param>
        protected static Expression<Func<TDocument, object>> ConvertExpression<TDocument, TValue>(Expression<Func<TDocument, TValue>> expression)
        {
            var param = expression.Parameters[0];
            Expression body = expression.Body;
            var convert = Expression.Convert(body, typeof(object));
            return Expression.Lambda<Func<TDocument, object>>(convert, param);
        }

        #endregion


    }
}