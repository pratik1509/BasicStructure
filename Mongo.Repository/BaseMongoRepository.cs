using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Common.Mongo.Repository
{
    /// <summary>
    /// The base Repository, it is meant to be inherited from by your custom custom MongoRepository implementation.
    /// Its constructor must be given a connection string and a database name.
    /// </summary>
    public abstract partial class BaseMongoRepository : ReadOnlyMongoRepository, IBaseMongoRepository
    {
        /// <summary>
        /// The constructor taking a connection string and a database name.
        /// </summary>
        /// <param name="connectionString">The connection string of the MongoDb server.</param>
        /// <param name="databaseName">The name of the database against which you want to perform operations.</param>
        protected BaseMongoRepository(string connectionString, string databaseName) : base(connectionString, databaseName)
        {
            MongoDbContext = new MongoDbContext(connectionString, databaseName);
        }

        /// <summary>
        /// The contructor taking a <see cref="IMongoDbContext"/>.
        /// </summary>
        /// <param name="mongoDbContext">A mongodb context implementing <see cref="IMongoDbContext"/></param>
        protected BaseMongoRepository(IMongoDbContext mongoDbContext) : base(mongoDbContext)
        {
            MongoDbContext = mongoDbContext;
        }

        /// <summary>
        /// The contructor taking a <see cref="IMongoDatabase"/>.
        /// </summary>
        /// <param name="mongoDatabase">A mongodb context implementing <see cref="IMongoDatabase"/></param>
        protected BaseMongoRepository(IMongoDatabase mongoDatabase) : base(mongoDatabase)
        {
            MongoDbContext = new MongoDbContext(mongoDatabase);
        }

        #region Create

        /// <summary>
        /// Asynchronously adds a document to the collection.
        /// Populates the Id and AddedAtUtc fields if necessary.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="document">The document you want to add.</param>
        public virtual async Task<string> AddOneAsync<TDocument>(TDocument document, string createdBy) where TDocument : IBaseModel
        {
            AddInsertProperties(document, createdBy);
            await GetCollection<TDocument>().InsertOneAsync(document);
            return document.Id;
        }

        /// <summary>
        /// Adds a document to the collection.
        /// Populates the Id and AddedAtUtc fields if necessary.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="document">The document you want to add.</param>
        public virtual string AddOne<TDocument>(TDocument document, string createdBy) where TDocument : IBaseModel
        {
            AddInsertProperties(document, createdBy);
            GetCollection<TDocument>().InsertOne(document);
            return document.Id;
        }

        /// <summary>
        /// Asynchronously adds a list of documents to the collection.
        /// Populates the Id and AddedAtUtc fields if necessary.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="documents">The documents you want to add.</param>
        public virtual async Task AddManyAsync<TDocument>(IEnumerable<TDocument> documents, string createdBy) where TDocument : IBaseModel
        {
            if (!documents.Any())
            {
                return;
            }
            foreach (var document in documents)
            {
                AddInsertProperties(document, createdBy);
            }

            await GetCollection<TDocument>().InsertManyAsync(documents.ToList());

        }

        /// <summary>
        /// Adds a list of documents to the collection.
        /// Populates the Id and AddedAtUtc fields if necessary.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="documents">The documents you want to add.</param>
        public virtual void AddMany<TDocument>(IEnumerable<TDocument> documents, string createdBy) where TDocument : IBaseModel
        {
            if (!documents.Any())
            {
                return;
            }
            foreach (var document in documents)
            {
                AddInsertProperties(document, createdBy);
            }
           
            GetCollection<TDocument>().InsertMany(documents.ToList());
        }

        #endregion Create

        #region Update

        /// <summary>
        /// Asynchronously Updates a document.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="modifiedDocument">The document with the modifications you want to persist.</param>
        public virtual async Task<bool> UpdateOneAsync<TDocument>(TDocument modifiedDocument, string updatedBy) where TDocument : IBaseModel
        {
            AddUpdateProperties(modifiedDocument, updatedBy);
            var collection = GetCollection<TDocument>();
            var updateRes = await collection.ReplaceOneAsync(x => x.Id == modifiedDocument.Id, modifiedDocument);
            return updateRes.ModifiedCount == 1;
        }

        /// <summary>
        /// Updates a document.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="modifiedDocument">The document with the modifications you want to persist.</param>
        public virtual bool UpdateOne<TDocument>(TDocument modifiedDocument, string updatedBy) where TDocument : IBaseModel
        {
            AddUpdateProperties(modifiedDocument, updatedBy);
            var collection = GetCollection<TDocument>();
            var updateRes = collection.ReplaceOne(x => x.Id == modifiedDocument.Id, modifiedDocument);
            return updateRes.ModifiedCount == 1;
        }

        /// <summary>
        /// Takes a document you want to modify and applies the update you have defined in MongoDb.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="documentToModify">The document you want to modify.</param>
        /// <param name="update">The update definition for the document.</param>
        public virtual async Task<bool> UpdateOneAsync<TDocument>(TDocument documentToModify, UpdateDefinition<TDocument> update, string updatedBy)
            where TDocument : IBaseModel
        {
            update = AddUpdateProperties(update, updatedBy);
            var filter = Builders<TDocument>.Filter.Eq("Id", documentToModify.Id);
            
            var updateRes = await GetCollection<TDocument>().UpdateOneAsync(filter, update);
            return updateRes.ModifiedCount == 1;
        }

        /// <summary>
        /// Updates the property field with the given value update a property field in entities.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="documentToModify">The document you want to modify.</param>
        /// <param name="field">The field selector.</param>
        /// <param name="value">The new value of the property field.</param>
        public virtual bool UpdateOne<TDocument, TField>(TDocument documentToModify, Expression<Func<TDocument, TField>> field, TField value, string updatedBy)
            where TDocument : IBaseModel
        {
            UpdateDefinition<TDocument> update = AddUpdateProperties(field, value, updatedBy);

            var filter = Builders<TDocument>.Filter.Eq("Id", documentToModify.Id);
            var updateRes = GetCollection<TDocument>().UpdateOne(filter, update);
            return updateRes.ModifiedCount == 1;
        }

        /// <summary>
        /// Updates the property field with the given value update a property field in entities.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="documentToModify">The document you want to modify.</param>
        /// <param name="field">The field selector.</param>
        /// <param name="value">The new value of the property field.</param>
        public virtual async Task<bool> UpdateOneAsync<TDocument, TField>(TDocument documentToModify, Expression<Func<TDocument, TField>> field, TField value, string updatedBy)
            where TDocument : IBaseModel
        {
            UpdateDefinition<TDocument> update = AddUpdateProperties(field, value, updatedBy);
            var filter = Builders<TDocument>.Filter.Eq("Id", documentToModify.Id);
            var updateRes = await GetCollection<TDocument>().UpdateOneAsync(filter, update);
            return updateRes.ModifiedCount == 1;
        }

        /// <summary>
        /// Updates the property field with the given value update a property field in entities.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="filter">The document filter.</param>
        /// <param name="field">The field selector.</param>
        /// <param name="value">The new value of the property field.</param>
        /// <param name="partitionKey">The value of the partition key.</param>
        public virtual bool UpdateOne<TDocument, TField>(FilterDefinition<TDocument> filter, Expression<Func<TDocument, TField>> field, TField value, string updatedBy, string partitionKey = null)
            where TDocument : IBaseModel
        {
            UpdateDefinition<TDocument> update = AddUpdateProperties(field, value, updatedBy);
            var collection = string.IsNullOrEmpty(partitionKey) ? GetCollection<TDocument>() : GetCollection<TDocument>(partitionKey);
            var updateRes = collection.UpdateOne(filter, update);
            return updateRes.ModifiedCount == 1;
        }

        /// <summary>
        /// For the entity selected by the filter, updates the property field with the given value.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="filter">The document filter.</param>
        /// <param name="field">The field selector.</param>
        /// <param name="value">The new value of the property field.</param>
        /// <param name="partitionKey">The partition key for the document.</param>
        public virtual bool UpdateOne<TDocument, TField>(Expression<Func<TDocument, bool>> filter, Expression<Func<TDocument, TField>> field, TField value, string updatedBy, string partitionKey = null)
            where TDocument : IBaseModel
        {
            UpdateDefinition<TDocument> update = AddUpdateProperties(field, value, updatedBy);
            var collection = string.IsNullOrEmpty(partitionKey) ? GetCollection<TDocument>() : GetCollection<TDocument>(partitionKey);
            var updateRes = collection.UpdateOne(Builders<TDocument>.Filter.Where(filter), update);
            return updateRes.ModifiedCount == 1;
        }

        /// <summary>
        /// Updates the property field with the given value update a property field in entities.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="filter">The document filter.</param>
        /// <param name="field">The field selector.</param>
        /// <param name="value">The new value of the property field.</param>
        /// <param name="partitionKey">The value of the partition key.</param>
        public virtual async Task<bool> UpdateOneAsync<TDocument, TField>(FilterDefinition<TDocument> filter, Expression<Func<TDocument, TField>> field, TField value, string updatedBy, string partitionKey = null)
            where TDocument : IBaseModel
        {
            UpdateDefinition<TDocument> update = AddUpdateProperties(field, value, updatedBy);
            var collection = string.IsNullOrEmpty(partitionKey) ? GetCollection<TDocument>() : GetCollection<TDocument>(partitionKey);
            var updateRes = await collection.UpdateOneAsync(filter, update);
            return updateRes.ModifiedCount == 1;
        }

        /// <summary>
        /// For the entity selected by the filter, updates the property field with the given value.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <typeparam name="TField">The type of the field.</typeparam>
        /// <param name="filter">The document filter.</param>
        /// <param name="field">The field selector.</param>
        /// <param name="value">The new value of the property field.</param>
        /// <param name="partitionKey">The partition key for the document.</param>
        public virtual async Task<bool> UpdateOneAsync<TDocument, TField>(Expression<Func<TDocument, bool>> filter, Expression<Func<TDocument, TField>> field, TField value, string updatedBy, string partitionKey = null)
            where TDocument : IBaseModel
        {
            UpdateDefinition<TDocument> update = AddUpdateProperties(field, value, updatedBy);
            var collection = string.IsNullOrEmpty(partitionKey) ? GetCollection<TDocument>() : GetCollection<TDocument>(partitionKey);
            var updateRes = await collection.UpdateOneAsync(Builders<TDocument>.Filter.Where(filter), update);
            return updateRes.ModifiedCount == 1;
        }

        /// <summary>
        /// Takes a document you want to modify and applies the update you have defined in MongoDb.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="documentToModify">The document you want to modify.</param>
        /// <param name="update">The update definition for the document.</param>
        public virtual bool UpdateOne<TDocument>(TDocument documentToModify, UpdateDefinition<TDocument> update, string updatedBy)
            where TDocument : IBaseModel
        {
            update = AddUpdateProperties(update, updatedBy);
            var filter = Builders<TDocument>.Filter.Eq("Id", documentToModify.Id);
            var updateRes = GetCollection<TDocument>().UpdateOne(filter, update, new UpdateOptions { IsUpsert = true });
            return updateRes.ModifiedCount == 1;
        }

        /// <summary>
        /// Asynchronosly update multiple docs
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <typeparam name="TField"></typeparam>
        /// <param name="filter"></param>     
        /// <param name="updatedBy"></param>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        public async Task<bool> UpdateManyAsync<TDocument>(Expression<Func<TDocument, bool>> filter,
            UpdateDefinition<TDocument> update, string updatedBy, string partitionKey = null)        
            where TDocument : IBaseModel
        {
            update = AddUpdateProperties(update, updatedBy);
            var collection = string.IsNullOrEmpty(partitionKey) ? GetCollection<TDocument>() : GetCollection<TDocument>(partitionKey);
            var updateRes = await collection.UpdateManyAsync(Builders<TDocument>.Filter.Where(filter), update);
            return updateRes.ModifiedCount >= 1;
        }

        /// <summary>
        /// Asynchronosly update multiple docs using filter defination
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <typeparam name="TField"></typeparam>
        /// <param name="filter"></param>     
        /// <param name="updatedBy"></param>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        public async Task<bool> UpdateManyAsync<TDocument>(FilterDefinition<TDocument> filter,
            UpdateDefinition<TDocument> update, string updatedBy, string partitionKey = null)
            where TDocument : IBaseModel
        {
            update = AddUpdateProperties(update, updatedBy);
            var collection = string.IsNullOrEmpty(partitionKey) ? GetCollection<TDocument>() : 
                GetCollection<TDocument>(partitionKey);

            var updateRes = await collection.UpdateManyAsync(filter, update);
            return updateRes.ModifiedCount >= 1;
        }

        /// <summary>
        /// Update list of documents
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="filter"></param>
        /// <param name="update"></param>
        /// <param name="updatedBy"></param>
        /// <returns></returns>
        public async Task<bool> UpdateManyAsyc<TDocument>(Expression<Func<TDocument, bool>> filter, UpdateDefinition<TDocument> update, string updatedBy)
           where TDocument : IBaseModel
        {           
            update = AddUpdateProperties(update, updatedBy);          
            var updateRes = await GetCollection<TDocument>().UpdateManyAsync(filter, update, new UpdateOptions { IsUpsert = false });
            return updateRes.ModifiedCount >= 1;
        }

        #endregion Update

        #region Project

        /// <summary>
        /// Asynchronously returns a projected document matching the filter condition.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <typeparam name="TProjection">The type representing the model you want to project to.</typeparam>
        /// <param name="filter"></param>
        /// <param name="projection">The projection expression.</param>
        /// <param name="partitionKey">An optional partition key.</param>
        public virtual async Task<TProjection> ProjectOneAsync<TDocument, TProjection>(Expression<Func<TDocument, bool>> filter, Expression<Func<TDocument, TProjection>> projection, string partitionKey = null)
            where TDocument : IBaseModel
            where TProjection : class
        {
            return await HandlePartitioned<TDocument>(partitionKey).Find(filter)
                                                                   .Project(projection)
                                                                   .FirstOrDefaultAsync();
        }
                
        /// <summary>
        /// Returns a projected document matching the filter condition.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <typeparam name="TProjection">The type representing the model you want to project to.</typeparam>
        /// <param name="filter"></param>
        /// <param name="projection">The projection expression.</param>
        /// <param name="partitionKey">An optional partition key.</param>
        public virtual TProjection ProjectOne<TDocument, TProjection>(Expression<Func<TDocument, bool>> filter, Expression<Func<TDocument, TProjection>> projection, string partitionKey = null)
            where TDocument : IBaseModel
            where TProjection : class
        {
            return HandlePartitioned<TDocument>(partitionKey).Find(filter)
                                                             .Project(projection)
                                                             .FirstOrDefault();
        }
        
        /// <summary>
        /// Asynchronously returns a list of projected documents matching the filter condition.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <typeparam name="TProjection">The type representing the model you want to project to.</typeparam>
        /// <param name="filter"></param>
        /// <param name="projection">The projection expression.</param>
        /// <param name="partitionKey">An optional partition key.</param>
        public virtual async Task<List<TProjection>> ProjectManyAsync<TDocument, TProjection>(Expression<Func<TDocument, bool>> filter, Expression<Func<TDocument, TProjection>> projection, string partitionKey = null)
            where TDocument : IBaseModel
            where TProjection : class
        {
            return await HandlePartitioned<TDocument>(partitionKey).Find(filter)
                                                                   .Project(projection)
                                                                   .ToListAsync();
        }
               
        /// <summary>
        /// Asynchronously returns a list of projected documents matching the filter condition.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <typeparam name="TProjection">The type representing the model you want to project to.</typeparam>
        /// <param name="filter"></param>
        /// <param name="projection">The projection expression.</param>
        /// <param name="partitionKey">An optional partition key.</param>
        public virtual List<TProjection> ProjectMany<TDocument, TProjection>(Expression<Func<TDocument, bool>> filter, Expression<Func<TDocument, TProjection>> projection, string partitionKey = null)
            where TDocument : IBaseModel
            where TProjection : class
        {
            return HandlePartitioned<TDocument>(partitionKey).Find(filter)
                                                             .Project(projection)
                                                             .ToList();
        }

        #endregion

        #region Grouping

        /// <summary>
        /// Groups a collection of documents given a grouping criteria, 
        /// and returns a dictionary of listed document groups with keys having the different values of the grouping criteria.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <typeparam name="TGroupKey">The type of the grouping criteria.</typeparam>
        /// <typeparam name="TProjection">The type of the projected group.</typeparam>
        /// <param name="groupingCriteria">The grouping criteria.</param>
        /// <param name="groupProjection">The projected group result.</param>
        /// <param name="partitionKey">The partition key of your document, if any.</param>
        public virtual List<TProjection> GroupBy<TDocument, TGroupKey, TProjection>(
            Expression<Func<TDocument, TGroupKey>> groupingCriteria,
            Expression<Func<IGrouping<TGroupKey, TDocument>, TProjection>> groupProjection,
            string partitionKey = null)
            where TDocument : IBaseModel
            where TProjection : class, new()
        {
            var collection = string.IsNullOrEmpty(partitionKey) ? GetCollection<TDocument>() : GetCollection<TDocument>(partitionKey);
            return collection.Aggregate()
                             .Group(groupingCriteria, groupProjection)
                             .ToList();

        }

        /// <summary>
        /// Groups filtered a collection of documents given a grouping criteria, 
        /// and returns a dictionary of listed document groups with keys having the different values of the grouping criteria.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <typeparam name="TGroupKey">The type of the grouping criteria.</typeparam>
        /// <typeparam name="TProjection">The type of the projected group.</typeparam>
        /// <param name="filter"></param>
        /// <param name="selector">The grouping criteria.</param>
        /// <param name="projection">The projected group result.</param>
        /// <param name="partitionKey">The partition key of your document, if any.</param>
        public virtual List<TProjection> GroupBy<TDocument, TGroupKey, TProjection>(Expression<Func<TDocument, bool>> filter,
                                                       Expression<Func<TDocument, TGroupKey>> selector,
                                                       Expression<Func<IGrouping<TGroupKey, TDocument>, TProjection>> projection,
                                                       string partitionKey = null)
                                                       where TDocument : IBaseModel
                                                       where TProjection : class, new()
        {
            var collection = string.IsNullOrEmpty(partitionKey) ? GetCollection<TDocument>() : GetCollection<TDocument>(partitionKey);
            return collection.Aggregate()
                             .Match(Builders<TDocument>.Filter.Where(filter))
                             .Group(selector, projection)
                             .ToList();
        }


        #endregion

        /// <summary>
        /// Asynchronously returns a paginated list of the documents matching the filter condition.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="filter"></param>
        /// <param name="skipNumber">The number of documents you want to skip. Default value is 0.</param>
        /// <param name="takeNumber">The number of documents you want to take. Default value is 50.</param>
        /// <param name="partitionKey">An optional partition key.</param>
        public virtual async Task<List<TDocument>> GetPaginatedAsync<TDocument>(Expression<Func<TDocument, bool>> filter, int skipNumber = 0, int takeNumber = 50, string partitionKey = null)
            where TDocument : IBaseModel
        {
            return await HandlePartitioned<TDocument>(partitionKey).Find(filter).Skip(skipNumber).Limit(takeNumber).ToListAsync();
        }

        #region Find And Update

        /// <summary>
        /// GetAndUpdateOne with filter
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="filter"></param>
        /// <param name="update"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public virtual async Task<TDocument> GetAndUpdateOne<TDocument>(FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, FindOneAndUpdateOptions<TDocument, TDocument> options, string updatedBy) where TDocument : IBaseModel
        {
            update = AddUpdateProperties(update, updatedBy);
            return await GetCollection<TDocument>().FindOneAndUpdateAsync(filter, update, options);
        }

        #endregion Find And Update

        #region SoftDelete
                
        public async Task<bool> DeleteOneAsync<TDocument>(Expression<Func<TDocument, bool>> filter, string deletedBy, string partitionKey = null) where TDocument : IBaseModel
        {
            UpdateDefinition<TDocument> update = AddDeleteProperties<TDocument>(deletedBy);
            
            var collection = string.IsNullOrEmpty(partitionKey) ? GetCollection<TDocument>() : GetCollection<TDocument>(partitionKey);
            var updateRes = await collection.UpdateOneAsync(filter, update);
            return updateRes.ModifiedCount == 1;
        }

        public bool DeleteOne<TDocument>(Expression<Func<TDocument, bool>> filter, string deletedBy, string partitionKey = null) where TDocument : IBaseModel
        {
            UpdateDefinition<TDocument> update = AddDeleteProperties<TDocument>(deletedBy);

            var collection = string.IsNullOrEmpty(partitionKey) ? GetCollection<TDocument>() : GetCollection<TDocument>(partitionKey);
            var updateRes = collection.UpdateOne(filter, update);
            return updateRes.ModifiedCount == 1;
        }

        public async Task<bool> DeleteManyAsync<TDocument>(Expression<Func<TDocument, bool>> filter, string deletedBy, string partitionKey = null) where TDocument : IBaseModel
        {
            UpdateDefinition<TDocument> update = AddDeleteProperties<TDocument>(deletedBy);

            var collection = string.IsNullOrEmpty(partitionKey) ? GetCollection<TDocument>() : GetCollection<TDocument>(partitionKey);
            var updateRes = await collection.UpdateManyAsync(filter, update);
            return updateRes.ModifiedCount >= 1;
        }

        public async Task<bool> DeleteManyAsync<TDocument>(FilterDefinition<TDocument> filter, string deletedBy, string partitionKey = null) where TDocument : IBaseModel
        {
            UpdateDefinition<TDocument> update = AddDeleteProperties<TDocument>(deletedBy);

            var collection = string.IsNullOrEmpty(partitionKey) ? GetCollection<TDocument>() : GetCollection<TDocument>(partitionKey);
            var updateRes = await collection.UpdateManyAsync(filter, update);
            return updateRes.ModifiedCount >= 1;
        }

        public long DeleteMany<TDocument>(Expression<Func<TDocument, bool>> filter, string deletedBy, string partitionKey = null) where TDocument : IBaseModel
        {
            throw new NotImplementedException();
        }

        #endregion

        #region HardDelete
        public Task<long> HardDeleteOneAsync<TDocument>(TDocument document) where TDocument : IBaseModel
        {
            throw new NotImplementedException();
        }

        public Task<long> HardDeleteOneAsync<TDocument>(Expression<Func<TDocument, bool>> filter, string partitionKey = null) where TDocument : IBaseModel
        {
            throw new NotImplementedException();
        }

        public long HardDeleteOne<TDocument>(TDocument document) where TDocument : IBaseModel
        {
            throw new NotImplementedException();
        }

        public long HardDeleteOne<TDocument>(Expression<Func<TDocument, bool>> filter, string partitionKey = null) where TDocument : IBaseModel
        {
            throw new NotImplementedException();
        }

        public Task<long> HardDeleteManyAsync<TDocument>(Expression<Func<TDocument, bool>> filter, string partitionKey = null) where TDocument : IBaseModel
        {
            throw new NotImplementedException();
        }

        public Task<long> HardDeleteManyAsync<TDocument>(IEnumerable<TDocument> documents) where TDocument : IBaseModel
        {
            throw new NotImplementedException();
        }

        public long HardDeleteMany<TDocument>(IEnumerable<TDocument> documents) where TDocument : IBaseModel
        {
            throw new NotImplementedException();
        }

        public long HardDeleteMany<TDocument>(Expression<Func<TDocument, bool>> filter, string partitionKey = null) where TDocument : IBaseModel
        {
            throw new NotImplementedException();
        }
        #endregion

        #region HelperMethods
       
        protected void AddInsertProperties<TDocument>(TDocument document, string createdBy) where TDocument : IBaseModel
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            document.CreatedOn = DateTime.UtcNow;
            document.CreatedBy = createdBy;

        }

        protected UpdateDefinition<TDocument> AddUpdateProperties<TDocument>(UpdateDefinition<TDocument> update, string modifiedBy) where TDocument : IBaseModel
        {
            update = update.Set(x => x.ModifiedOn, DateTime.UtcNow)
                .Set(x => x.ModifiedBy, modifiedBy);

            return update;
        }

        protected TDocument AddUpdateProperties<TDocument>(TDocument modifiedDocument, string modifiedBy) where TDocument : IBaseModel
        {
            modifiedDocument.ModifiedOn = DateTime.UtcNow;
            modifiedDocument.ModifiedBy = modifiedBy;
            return modifiedDocument;
        }

        protected UpdateDefinition<TDocument> AddUpdateProperties<TDocument, TField>(Expression<Func<TDocument, TField>> field, TField value, string modifiedBy) where TDocument : IBaseModel
        {
            UpdateDefinition<TDocument> update;
            update = Builders<TDocument>.Update.Set(x => x.ModifiedOn, DateTime.UtcNow)
                                                .Set(x => x.ModifiedBy, modifiedBy)
                                                .Set(field, value);

            return update;
        }

        protected UpdateDefinition<TDocument> AddDeleteProperties<TDocument>(string deletedBy) where TDocument : IBaseModel
        {
            UpdateDefinition<TDocument> update = Builders<TDocument>.Update
                .Set(x => x.IsDeleted, true)
                .Set(x => x.DeletedOn, DateTime.UtcNow)
                .Set(x => x.DeletedBy, deletedBy);
            return update;
        }

        #endregion
    }
}