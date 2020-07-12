using MessageCenter.Business;
using MongoDB.Bson;
using SSO.Util.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogService
{
    public class Processor
    {
        public List<Task> tasks = new List<Task>();
        public Processor() { }
        public void StartWork()
        {
            Task task = Task.Factory.StartNew(() =>
            {
                MsQueue<LogModel> msQueue = new MsQueue<LogModel>(AppSettings.GetValue("log_msqueue"));
                msQueue.ReceiveMessage(Worker);
            });
            tasks.Add(task);
        }
        public void Worker(LogModel logModel)
        {
            LogModelTimer logModelTimer = new LogModelTimer(logModel);
            string key = logModelTimer.GetLogKey();
            if (!LogStorage.Logs.ContainsKey(key))
            {
                bool result = LogStorage.Logs.TryAdd(key, logModelTimer);
            }
            else
            {
                LogStorage.Logs[key].LogModel.CountPerMinute++;
            }
        }
    }
}
