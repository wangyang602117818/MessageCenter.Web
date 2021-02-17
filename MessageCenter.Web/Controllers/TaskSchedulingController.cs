using MessageCenter.Web.Models;
using SSO.Util.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MessageCenter.Web.Controllers
{
    public class TaskSchedulingController : Controller
    {
        protected string task_scheduling_msqueue = AppSettings.GetValue("task_scheduling_msqueue");
        protected string task_scheduling_remote_msqueue = AppSettings.GetValue("task_scheduling_remote_msqueue");
        public ActionResult Insert(SchedulingQueueModel schedulingQueueModel)
        {
            MsQueue<SchedulingQueueModel> schedulingQueue = null;
            if (schedulingQueueModel.MachineName.IsNullOrEmpty())
            {
                schedulingQueue = new MsQueue<SchedulingQueueModel>(task_scheduling_msqueue);
            }
            else
            {
                schedulingQueue = new MsQueue<SchedulingQueueModel>(task_scheduling_remote_msqueue.Replace("{machineName}", schedulingQueueModel.MachineName));
            }
            schedulingQueue.SendMessage(schedulingQueueModel, "task_scheduling");
            return new ResponseModel<string>(ErrorCode.success, "");
        }
    }
}