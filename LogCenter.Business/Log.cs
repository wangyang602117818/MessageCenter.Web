using MongoDB.Bson;
using MongoDB.Driver;
using SSO.Util.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogCenter.Business
{
    public class Log : ModelBase<Data.Log>
    {
        public Log() : base(new Data.Log()) { }
        public IEnumerable<BsonDocument> GetFromList()
        {
            return mongoData.GetFromList().Select(s => { s["from"] = s["_id"]; s.Remove("_id"); return s; });
        }
        public IEnumerable<BsonDocument> GetPageList(ref long count, string from, string controller, string action = null, DateTime? startTime = null, DateTime? endTime = null, string userId = null, Dictionary<string, string> sorts = null, int pageIndex = 1, int pageSize = 10)
        {
            return mongoData.GetPageList(ref count, from, controller, action, startTime, endTime, userId, sorts, pageIndex, pageSize);
        }
        public IEnumerable<BsonDocument> OpRecordDay(DateTime createTime)
        {
            return mongoData.OpRecordDay(createTime).Select(s => { s["date"] = s["_id"]; s.Remove("_id"); return s; });
        }
    }
}
