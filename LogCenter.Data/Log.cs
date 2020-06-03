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
        public IEnumerable<BsonDocument> GetPageList(ref long count, string from, string controller, string action = null, DateTime? startTime = null, DateTime? endTime = null, string userId = null, Dictionary<string, string> sorts = null, int pageIndex = 1, int pageSize = 10)
        {
            FilterDefinition<BsonDocument> filterBuilder = new BsonDocument();
            List<FilterDefinition<BsonDocument>> list = new List<FilterDefinition<BsonDocument>>();
            if (!from.IsNullOrEmpty()) list.Add(FilterBuilder.Eq("From", from));
            if (!controller.IsNullOrEmpty()) list.Add(FilterBuilder.Eq("Controller", controller.ToLower()));
            if (!action.IsNullOrEmpty()) list.Add(FilterBuilder.Eq("Action", action.ToLower()));
            if (startTime != null) list.Add(FilterBuilder.Gte("CreateTime", startTime.Value));
            if (endTime != null) list.Add(FilterBuilder.Lte("CreateTime", endTime.Value.AddDays(1)));
            if (!userId.IsNullOrEmpty()) list.Add(FilterBuilder.Eq("UserId", userId.ToLower()));
            if (list.Count > 0) filterBuilder = FilterBuilder.And(list);
            var find = MongoCollection.Find(filterBuilder);
            count = find.CountDocuments();
            if (sorts != null)
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
