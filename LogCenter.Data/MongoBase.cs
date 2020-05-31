using MongoDB.Bson;
using MongoDB.Driver;
using SSO.Util.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LogCenter.Data
{
    public class MongoBase
    {
        protected IMongoDatabase MongoDatabase;
        protected IMongoCollection<BsonDocument> MongoCollection;
        protected FilterDefinitionBuilder<BsonDocument> FilterBuilder = Builders<BsonDocument>.Filter;
        public MongoBase(string collectionName)
        {
            MongoDatabase = MongoDataSource.MongoClient.GetDatabase(AppSettings.GetValue("database"));
            MongoCollection = MongoDatabase.GetCollection<BsonDocument>(collectionName);
        }
        public void Insert(BsonDocument document)
        {
            MongoCollection.InsertOne(document);
        }
        public async void InsertOneAsync(BsonDocument document)
        {
            await MongoCollection.InsertOneAsync(document);
        }
        public void InsertManyAsync(IEnumerable<BsonDocument> documents)
        {
            MongoCollection.InsertManyAsync(documents);
        }
        public bool Update(ObjectId id, BsonDocument document)
        {
            return MongoCollection.UpdateOne(FilterBuilder.Eq("_id", id), new BsonDocument("$set", document)).IsAcknowledged;
        }
        public bool DeleteOne(ObjectId id)
        {
            return MongoCollection.DeleteOne(new BsonDocument("_id", id)).IsAcknowledged;
        }
        public bool DeleteMany(IEnumerable<ObjectId> ids)
        {
            return MongoCollection.DeleteMany(FilterBuilder.In("_id", ids)).IsAcknowledged;
        }
        public IEnumerable<BsonDocument> Find(BsonDocument document)
        {
            return MongoCollection.Find(document).ToEnumerable();
        }
        public BsonDocument FindOne(ObjectId id)
        {
            return MongoCollection.Find(new BsonDocument("_id", id)).FirstOrDefault();
        }
        public IEnumerable<BsonDocument> FindByIds(IEnumerable<ObjectId> ids)
        {
            return MongoCollection.Find(FilterBuilder.In("_id", ids)).ToEnumerable();
        }
        public BsonDocument FindOneNotDelete(ObjectId id)
        {
            var filter = FilterBuilder.Eq("_id", id) & FilterBuilder.Eq("Delete", false);
            return MongoCollection.Find(filter).FirstOrDefault();
        }
        public IEnumerable<BsonDocument> FindAll()
        {
            return MongoCollection.Find(new BsonDocument(), new FindOptions() { BatchSize = 1000 }).ToEnumerable();
        }
        public virtual long Count()
        {
            return MongoCollection.EstimatedDocumentCount();
        }
        
    }
}
