using MongoDB.Bson;
using SSO.Util.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogCenter.Business
{
    public class Processor
    {
        public List<Task> tasks = new List<Task>();
        public Processor() { }
        public void StartWork()
        {
            Task task = Task.Factory.StartNew(() =>
            {
                MsQueue<LogModel> msQueue = new MsQueue<LogModel>(AppSettings.GetValue("msqueue"));
                msQueue.ReceiveMessage(Worker);
            });
            tasks.Add(task);
        }
        public void Worker(LogModel logModel)
        {
            try
            {
                new Log().InsertOneAsync(logModel.ToBsonDocument());
            }
            catch (Exception ex)
            {
                Log4Net.ErrorLog(ex);
            }
        }
    }
}
