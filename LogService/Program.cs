using SSO.Util.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace LogService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            var assembly = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + "SSO.Util.Client.dll");
            var stream = assembly.GetManifestResourceStream("SSO.Util.Client.log4net.config");
            log4net.Config.XmlConfigurator.Configure(stream);

            MsQueue<LogModel> msQueue = new MsQueue<LogModel>(AppSettings.GetValue("msqueue"));
            msQueue.CreateQueue();

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Service1()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
