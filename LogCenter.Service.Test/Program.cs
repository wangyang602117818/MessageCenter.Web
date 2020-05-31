using LogCenter.Business;
using SSO.Util.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LogCenter.Service.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var assembly = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + "SSO.Util.Client.dll");
            var stream = assembly.GetManifestResourceStream("SSO.Util.Client.log4net.config");
            log4net.Config.XmlConfigurator.Configure(stream);

            Log4Net.InfoLog("start...");
            new Processor().StartWork();

            Console.WriteLine("ok");
            Console.ReadKey();
        }
    }
}
