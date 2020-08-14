using MessageCenter.Web.Models;
using SSO.Util.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MessageCenter.Web.Controllers
{
    public class FileTaskController : Controller
    {
        protected string file_msqueue = AppSettings.GetValue("file_msqueue");
        protected string file_remote_msqueue = AppSettings.GetValue("file_remote_msqueue");
        public ActionResult Insert(FileConvertModel fileConvertModel)
        {
            MsQueue<FileConvertModel> convertMsQueue = null;
            if (fileConvertModel.MachineName.IsNullOrEmpty())
            {
                convertMsQueue = new MsQueue<FileConvertModel>(file_msqueue);
            }
            else
            {
                convertMsQueue = new MsQueue<FileConvertModel>(file_remote_msqueue.Replace("{machineName}", fileConvertModel.MachineName));
            }
            convertMsQueue.SendMessage(fileConvertModel, "task");
            return new ResponseModel<string>(ErrorCode.success, "");
        }
    }
}