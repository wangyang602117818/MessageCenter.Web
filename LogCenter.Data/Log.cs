using MongoDB.Bson;
using MongoDB.Driver;
using SSO.Util.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogCenter.Data
{
    public class Log : MongoBase
    {
        public Log() : base("Log_" + DateTime.Now.Year) { }
        public IEnumerable<BsonDocument> GetFromList()
        {
            return MongoCollection.Aggregate()
                .Group<BsonDocument>(new BsonDocument()
                {
                    {"_id","$From" },
                    {"count",new BsonDocument("$sum",1) }
                }).ToEnumerable();
        }
        public IEnumerable<BsonDocument> OpRecordDay(DateTime createTime)
        {
            return MongoCollection.Aggregate()
                 .Match(FilterBuilder.Gte("CreateTime", createTime))
                 .Project(new BsonDocument("date", new BsonDocument("$dateToString", new BsonDocument() {
                    {"format", "%Y-%m-%d" },
                    {"timezone",AppSettings.GetValue("timezone") },
                    {"date", "$CreateTime" }}
                 )))
                 .Group<BsonDocument>(new BsonDocument() {
                    {"_id","$date" },
                    {"count",new BsonDocument("$sum",1) }
                 })
                 .Sort(new BsonDocument("_id", 1)).ToEnumerable();
        }
    }
}
