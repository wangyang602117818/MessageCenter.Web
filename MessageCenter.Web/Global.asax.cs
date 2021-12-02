using MessageCenter.Web.Models;
using SSO.Util.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MessageCenter.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            MsQueue<LogModel> logMsQueue = new MsQueue<LogModel>(AppSettings.GetValue("log_msqueue"));
            logMsQueue.CreateQueue();

            MsQueue<FileConvertModel> fileTaskMsQueue = new MsQueue<FileConvertModel>(AppSettings.GetValue("file_msqueue"));
            fileTaskMsQueue.CreateQueue();

            MsQueue<string> taskSchedulingMsqueue = new MsQueue<string>(AppSettings.GetValue("task_scheduling_msqueue"));
            taskSchedulingMsqueue.CreateQueue();

            MsQueue<string> searchDataMsqueue = new MsQueue<string>(AppSettings.GetValue("search_data_msqueue"));
            searchDataMsqueue.CreateQueue(true);
        }
    }
}
