using SSO.Util.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace LogCenter.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            var assembly = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + "bin\\SSO.Util.Client.dll");
            var stream = assembly.GetManifestResourceStream("SSO.Util.Client.log4net.config");
            log4net.Config.XmlConfigurator.Configure(stream);

            MsQueue<LogModel> msQueue = new MsQueue<LogModel>(AppSettings.GetValue("msqueue"));
            msQueue.CreateQueue();
        }
    }
}
