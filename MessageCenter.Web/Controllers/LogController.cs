using MessageCenter.Business;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using SSO.Util.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace MessageCenter.Web.Controllers
{
    public class LogController : Controller
    {
        protected MsQueue<LogModel> logMsQueue = new MsQueue<LogModel>(AppSettings.GetValue("log_msqueue"));
        protected Log log = new Log();
        public ActionResult Insert(LogModel logModel)
        {
            logMsQueue.SendMessage(logModel, "log");
            return new ResponseModel<string>(ErrorCode.success, "");
        }
        [OutputCache(Duration = 60 * 60 * 4)]
        public ActionResult GetFromList()
        {
            var result = log.GetFromList().ToJson().ReplaceHttpPrefix();
            return new ResponseModel<string>(ErrorCode.success, result);
        }
        [OutputCache(Duration = 60 * 60 * 4)]
        public ActionResult GetToList()
        {
            var result = log.GetToList().ToJson().ReplaceHttpPrefix();
            return new ResponseModel<string>(ErrorCode.success, result);
        }
        [OutputCache(Duration = 60 * 60 * 4, VaryByParam = "*")]
        public ActionResult GetControllersByTo(string to)
        {
            var result = log.GetControllersByFrom(to).ToJson();
            return new ResponseModel<string>(ErrorCode.success, result);
        }
        [OutputCache(Duration = 60 * 60 * 4, VaryByParam = "*")]
        public ActionResult GetActionsByController(string to, string controllerName)
        {
            var result = log.GetActionsByController(to, controllerName).ToJson();
            return new ResponseModel<string>(ErrorCode.success, result);
        }
        [HttpPost]
        public ActionResult GetList(LogListModel logModel)
        {
            long count = 0;
            var filter = log.GetLogFilter(logModel.From, logModel.To, logModel.ControllerName, logModel.ActionName, logModel.StartTime, logModel.EndTime, logModel.UserId, logModel.UserName, logModel.Exception);
            var result = log.GetPageList(filter, null, null, logModel.Sorts, logModel.PageIndex, logModel.PageSize, ref count).ToJson().ReplaceJsonString();
            return new ResponseModel<string>(ErrorCode.success, result, count);
        }
        public ActionResult GetById(string id)
        {
            var result = log.FindOne(ObjectId.Parse(id)).ToJson().ReplaceJsonString();
            return new ResponseModel<string>(ErrorCode.success, result);
        }
        /// <summary>
        /// 最近几天操作记录
        /// </summary>
        /// <param name="last"></param>
        /// <returns></returns>
        public ActionResult RecordByDay(int last = 30)
        {
            var result = log.OpRecordDay(DateTime.Now.AddDays(-last)).ToJson();
            return new ResponseModel<string>(ErrorCode.success, result);
        }
        [OutputCache(Duration = 60 * 60, VaryByParam = "*")]
        public ActionResult GetOperations()
        {
            //昨日操作数
            var now = DateTime.UtcNow;
            long lastDay = log.GetCountByDate(now.AddDays(-1).Date, now.Date);
            //上月操作数
            var lastMonthStart = new DateTime(now.Year, now.Month - 1, 1, 0, 0, 0);
            var lastMonthEnd = new DateTime(now.Year, now.Month, 1, 0, 0, 0);
            long lastMonth = log.GetCountByDate(lastMonthStart, lastMonthEnd);
            //总操作数
            long all = log.GetCountByDate(null, null);
            return new ResponseModel<object>(ErrorCode.success, new { lastDay, lastMonth, all });
        }

    }


}