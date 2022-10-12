
using MessageCenter.Business;
using MongoDB.Bson;
using SSO.Util.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogService
{
    public class LogStorage
    {
        public static ConcurrentDictionary<string, LogModelTimer> Logs = new ConcurrentDictionary<string, LogModelTimer>();
    }
    public class LogModelTimer
    {
        Timer timer = null;
        public LogModel LogModel { get; set; }
        public LogModelTimer(LogModel logModel)
        {
            LogModel = logModel;
            timer = new Timer(timeoutCallback, this, 1000 * 60, Timeout.Infinite);
        }
        public string GetLogKey()
        {
            string key = LogModel.To + LogModel.Controller + LogModel.Action + LogModel.UserHost + LogModel.CreateTime.ToString("yyyyMMddHHmm");
            return key.ToLower();
        }
        /// <summary>
        /// 日志写入数据库
        /// </summary>
        /// <param name="state"></param>
        void timeoutCallback(object state)
        {
            LogModelTimer logModelTimer = (LogModelTimer)state;
            var key = logModelTimer.GetLogKey();
            if (LogStorage.Logs.TryRemove(key, out LogModelTimer outLog))
            {
                BsonDocument log = outLog.LogModel.ToBsonDocument();
                log.Remove("_id");
                new Log().Insert(log);
                outLog.timer.Dispose();
            }
        }
    }
}
