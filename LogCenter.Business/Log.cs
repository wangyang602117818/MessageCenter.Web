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
        public void InsertOneAsync(LogModel logModel)
        {
            mongoData.InsertOneAsync(logModel.ToBsonDocument());
        }
        public IEnumerable<BsonDocument> GetPageList(ref long count, string from, string userId, Dictionary<string, string> sorts = null, int pageIndex = 1, int pageSize = 10)
        {
            return mongoData.GetPageList(ref count, from, userId, sorts, pageIndex, pageSize);
        }
        public IEnumerable<BsonDocument> OpRecordDay(DateTime createTime)
        {
            return mongoData.OpRecordDay(createTime).Select(s => { s["date"] = s["_id"]; s.Remove("_id"); return s; });
        }
    }
}
