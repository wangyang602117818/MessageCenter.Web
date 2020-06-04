using MongoDB.Bson;
using MongoDB.Driver;
using SSO.Util.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LogCenter.Business
{
    public class Log : ModelBase<Data.Log>
    {
        public Log() : base(new Data.Log()) { }
        public FilterDefinition<BsonDocument> GetLogFilter(string from, string controller, string action = null, DateTime? startTime = null, DateTime? endTime = null, string userId = null, string userName = null)
        {
            FilterDefinition<BsonDocument> filterBuilder = new BsonDocument();
            List<FilterDefinition<BsonDocument>> list = new List<FilterDefinition<BsonDocument>>();
            if (!from.IsNullOrEmpty()) list.Add(FilterBuilder.Regex("From", new Regex("^" + from + "$", RegexOptions.IgnoreCase)));
            if (!controller.IsNullOrEmpty()) list.Add(FilterBuilder.Regex("Controller", new Regex("^" + controller + "$", RegexOptions.IgnoreCase)));
            if (!action.IsNullOrEmpty()) list.Add(FilterBuilder.Regex("Action", new Regex("^" + action + "$", RegexOptions.IgnoreCase)));
            if (startTime != null) list.Add(FilterBuilder.Gte("CreateTime", startTime.Value));
            if (endTime != null) list.Add(FilterBuilder.Lte("CreateTime", endTime.Value.AddDays(1)));
            if (!userId.IsNullOrEmpty()) list.Add(FilterBuilder.Regex("UserId", new Regex("^" + userId + "$", RegexOptions.IgnoreCase)));
            if (!userName.IsNullOrEmpty()) list.Add(FilterBuilder.Regex("UserName", new Regex("^" + userName + "$", RegexOptions.IgnoreCase)));
            if (list.Count > 0) filterBuilder = FilterBuilder.And(list);
            return filterBuilder;
        }
        public IEnumerable<BsonDocument> GetFromList()
        {
            return mongoData.GetFromList().Select(s => { s["from"] = s["_id"]; s.Remove("_id"); return s; });
        }
        public IEnumerable<BsonDocument> OpRecordDay(DateTime createTime)
        {
            return mongoData.OpRecordDay(createTime).Select(s => { s["date"] = s["_id"]; s.Remove("_id"); return s; });
        }
    }
}
