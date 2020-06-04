using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogCenter.Business
{
    public class ModelBase<T> where T : Data.MongoBase
    {
        [BsonIgnore]
        protected T mongoData;
        protected FilterDefinitionBuilder<BsonDocument> FilterBuilder = Builders<BsonDocument>.Filter;
        public ModelBase(T mongoData)
        {
            this.mongoData = mongoData;
        }
        public void Insert(BsonDocument document)
        {
            mongoData.Insert(document);
        }
        public void InsertOneAsync(BsonDocument document)
        {
            mongoData.InsertOneAsync(document);
        }
        public void InsertManyAsync(IEnumerable<BsonDocument> documents)
        {
            mongoData.InsertManyAsync(documents);
        }
        public bool Update(ObjectId id, BsonDocument document)
        {
            return mongoData.Update(id, document);
        }
        public bool DeleteOne(ObjectId id)
        {
            return mongoData.DeleteOne(id);
        }
        public bool DeleteMany(IEnumerable<ObjectId> ids)
        {
            return mongoData.DeleteMany(ids);
        }
        public IEnumerable<BsonDocument> Find(BsonDocument document)
        {
            return mongoData.Find(document);
        }
        public IEnumerable<BsonDocument> FindByIds(IEnumerable<ObjectId> ids)
        {
            return mongoData.FindByIds(ids);
        }
        public BsonDocument FindOne(ObjectId id)
        {
            return mongoData.FindOne(id);
        }
        public IEnumerable<BsonDocument> GetPageList(FilterDefinition<BsonDocument> eqs, IEnumerable<string> excludeFields, IEnumerable<string> includeFields, Dictionary<string, string> sorts, int pageIndex, int pageSize, ref long count)
        {
            return mongoData.GetPageList(eqs, excludeFields, includeFields, sorts, pageIndex, pageSize, ref count);
        }
    }
}
