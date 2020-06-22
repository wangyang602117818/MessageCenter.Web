using MongoDB.Bson;
using MongoDB.Driver;
using SSO.Util.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MessageCenter.Data
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
        public IEnumerable<BsonDocument> FindAll()
        {
            return MongoCollection.Find(new BsonDocument(), new FindOptions() { BatchSize = 1000 }).ToEnumerable();
        }
        public virtual long Count()
        {
            return MongoCollection.EstimatedDocumentCount();
        }
        public IEnumerable<BsonDocument> GetPageList(FilterDefinition<BsonDocument> eqs, IEnumerable<string> excludeFields, IEnumerable<string> includeFields, Dictionary<string, string> sorts, int pageIndex, int pageSize, ref long count)
        {
            count = MongoCollection.CountDocuments(eqs);
            var find = MongoCollection.Find(eqs);
            //排除字段
            if (excludeFields != null && excludeFields.Count() > 0)
            {
                ProjectionDefinition<BsonDocument> exclude = null;
                foreach (string ex in excludeFields)
                {
                    if (exclude == null) exclude = Builders<BsonDocument>.Projection.Exclude(ex);
                    exclude = exclude.Exclude(ex);
                }
                find = find.Project(exclude);
            }
            //包含字段
            if (includeFields != null && includeFields.Count() > 0)
            {
                ProjectionDefinition<BsonDocument> include = null;
                foreach (string ex in includeFields)
                {
                    if (include == null) include = Builders<BsonDocument>.Projection.Include(ex);
                    include = include.Include(ex);
                }
                find = find.Project(include);
            }
            if (sorts != null && sorts.Count > 0)
            {
                foreach (var item in sorts)
                {
                    if (item.Value == "asc") find = find.SortBy(sort => sort[item.Key]);
                    if (item.Value == "desc") find = find.SortByDescending(sort => sort[item.Key]);
                }
            }
            return find.Skip((pageIndex - 1) * pageSize)
                .Limit(pageSize)
                .ToEnumerable();
        }
    }
}
