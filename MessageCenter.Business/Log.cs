using MongoDB.Bson;
using MongoDB.Driver;
using SSO.Util.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MessageCenter.Business
{
    public class Log : ModelBase<Data.Log>
    {
        public Log() : base(new Data.Log()) { }
        public FilterDefinition<BsonDocument> GetLogFilter(string from, string controller, string action = null, DateTime? startTime = null, DateTime? endTime = null, string userId = null, string userName = null, bool? exception = null)
        {
            FilterDefinition<BsonDocument> filterBuilder = new BsonDocument();
            List<FilterDefinition<BsonDocument>> list = new List<FilterDefinition<BsonDocument>>();
            if (!from.IsNullOrEmpty()) list.Add(FilterBuilder.Regex("From", new Regex("^" + from + "$", RegexOptions.IgnoreCase)));
            if (!controller.IsNullOrEmpty()) list.Add(FilterBuilder.Regex("Controller", new Regex("^" + controller + "$", RegexOptions.IgnoreCase)));
            if (!action.IsNullOrEmpty()) list.Add(FilterBuilder.Regex("Action", new Regex("^" + action + "$", RegexOptions.IgnoreCase)));
            if (startTime != null) list.Add(FilterBuilder.Gte("CreateTime", startTime.Value));
            if (endTime != null) list.Add(FilterBuilder.Lte("CreateTime", endTime.Value.AddDays(1)));
            if (!userId.IsNullOrEmpty()) list.Add(FilterBuilder.Regex("UserId", new Regex("^" + userId, RegexOptions.IgnoreCase)));
            if (!userName.IsNullOrEmpty()) list.Add(FilterBuilder.Regex("UserName", new Regex("^" + userName, RegexOptions.IgnoreCase)));
            if (exception != null) list.Add(FilterBuilder.Eq("Exception", exception.Value));
            if (list.Count > 0) filterBuilder = FilterBuilder.And(list);
            return filterBuilder;
        }
        public IEnumerable<BsonDocument> GetFromList()
        {
            return mongoData.GetFromList().Select(s => { s["from"] = s["_id"]; s.Remove("_id"); return s; });
        }
        public IEnumerable<BsonDocument> GetToList()
        {
            return mongoData.GetToList().Select(s => { s["to"] = s["_id"]; s.Remove("_id"); return s; });
        }
        public IEnumerable<BsonDocument> OpRecordDay(DateTime createTime)
        {
            return mongoData.OpRecordDay(createTime).Select(s => { s["date"] = s["_id"]; s.Remove("_id"); return s; });
        }
        public IEnumerable<BsonDocument> GetControllersByFrom(string to)
        {
            return mongoData.GetControllersByFrom(to).Select(s => { s["controller"] = s["_id"]; s.Remove("_id"); return s; });
        }
        public IEnumerable<BsonDocument> GetActionsByController(string to, string controller)
        {
            return mongoData.GetActionsByController(to, controller).Select(s => { s["action"] = s["_id"]; s.Remove("_id"); return s; });
        }
        public long GetCountByDate(DateTime? startTime, DateTime? endTime)
        {
            return mongoData.GetCountByDate(startTime, endTime);
        }
    }
}
