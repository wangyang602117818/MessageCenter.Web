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
        public IEnumerable<BsonDocument> GetPageList(ref long count, string from, int? type, string userId, Dictionary<string, string> sorts = null, int pageIndex = 1, int pageSize = 10)
        {
            FilterDefinition<BsonDocument> filterBuilder = new BsonDocument();
            List<FilterDefinition<BsonDocument>> list = new List<FilterDefinition<BsonDocument>>();
            if (!from.IsNullOrEmpty()) list.Add(FilterBuilder.Eq("From", from));
            if (type != null) list.Add(FilterBuilder.Eq("Type", type));
            if (!userId.IsNullOrEmpty()) list.Add(FilterBuilder.Eq("UserId", userId));
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
    }
}
