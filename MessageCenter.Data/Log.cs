using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using SSO.Util.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageCenter.Data
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
        public IEnumerable<BsonDocument> GetToList()
        {
            return MongoCollection.Aggregate()
               .Group<BsonDocument>(new BsonDocument()
               {
                    {"_id","$To" },
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
        public IEnumerable<BsonDocument> GetControllersByFrom(string to)
        {
            return MongoCollection.Aggregate()
                .Match(FilterBuilder.Eq("To", to))
                .Group<BsonDocument>(new BsonDocument()
                {
                    {"_id","$Controller" },
                    {"count",new BsonDocument("$sum",1) }
                }).ToEnumerable();
        }
        public IEnumerable<BsonDocument> GetActionsByController(string to, string controller)
        {
            return MongoCollection.Aggregate()
                .Match(FilterBuilder.Eq("To", to) & FilterBuilder.Eq("Controller", controller))
                .Group<BsonDocument>(new BsonDocument()
                {
                    {"_id","$Action" },
                    {"count",new BsonDocument("$sum",1) }
                }).ToEnumerable();
        }
        public long GetCountByDate(DateTime? startTime, DateTime? endTime)
        {
            List<FilterDefinition<BsonDocument>> list = new List<FilterDefinition<BsonDocument>>();
            if (startTime != null) list.Add(FilterBuilder.Gte("CreateTime", startTime.Value));
            if (endTime != null) list.Add(FilterBuilder.Lte("CreateTime", endTime.Value));
            if (list.Count == 0) return MongoCollection.EstimatedDocumentCount();
            return MongoCollection.CountDocuments(FilterBuilder.And(list));
        }
        public void Watch()
        {
            var bs = new BsonDocument("_data", "82612F5A88000000012B022C0100296E5A10040D21CA985104416DA78D8648D5BFCD8046645F69640064612F5A8810F79F0C39BD77190004");
            var options = new ChangeStreamOptions { FullDocument = ChangeStreamFullDocumentOption.UpdateLookup, ResumeAfter = bs };
            var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<BsonDocument>>().Match("{ operationType: { $in: [ 'insert', 'delete','update','replace' ] } }");
            var cursor = MongoDatabase.GetCollection<BsonDocument>("t01").Watch(pipeline, options);
            while (cursor.MoveNext())
            {
                var doc = cursor.Current;
                if (doc.Count() > 0)
                {
                    foreach (var d in doc)
                    {
                        string id = d.BackingDocument["_id"]["_data"].ToString();
                        Console.WriteLine(d.BackingDocument.ToJson(new JsonWriterSettings() { OutputMode = JsonOutputMode.RelaxedExtendedJson }));
                    }
                }
            }
        }
        public IEnumerable<BsonDocument> GetLastValuableRecord()
        {

            return null;
        }
    }
}
