using LogCenter.Business;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using SSO.Util.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace LogCenter.Web.Controllers
{
    public class LogController : Controller
    {
        protected MsQueue<LogModel> msQueue = new MsQueue<LogModel>(AppSettings.GetValue("msqueue"));
        protected Log log = new Log();
        public ActionResult Insert(LogModel logModel)
        {
            msQueue.SendMessage(logModel, "log");
            return new ResponseModel<string>(ErrorCode.success, "");
        }
        public ActionResult GetList(string from = null, int? type = null, string userId = null, Dictionary<string, string> sorts = null, int pageIndex = 1, int pageSize = 10)
        {
            long count = 0;
            var result = log.GetPageList(ref count, from, type, userId, sorts, pageIndex, pageSize).ToJson().ReplaceJsonString();
            return new ResponseModel<string>(ErrorCode.success, result, count);
        }
    }


}