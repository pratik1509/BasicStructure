using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;
using System;

namespace Mongo.Repository
{
    /// <summary>
    /// This interface is being refrenced from Mongo.Repository. Do not delete it.
    /// </summary>
    public interface IBaseModel
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfDefault]
        string Id { get; set; }
        string CreatedBy { get; set; }
        DateTime CreatedOn { get; set; }       
        bool IsDeleted { get; set; }
        string ModifiedBy { get; set; }
        DateTime ModifiedOn { get; set; }
        string DeletedBy { get; set; }
        DateTime DeletedOn { get; set; }
    }
}