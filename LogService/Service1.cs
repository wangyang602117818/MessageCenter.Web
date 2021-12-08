using MessageCenter.Business;
using SSO.Util.Client;
using SSO.Util.Client.ElasticLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace LogService
{
    public partial class Service1 : ServiceBase
    {
        protected Processor processor = new Processor();
        protected SearchProcessor searchProcessor = new SearchProcessor();
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Log4Net.InfoLog("start...");
            processor.StartWork();
            searchProcessor.StartWork();
        }

        protected override void OnStop()
        {
            Log4Net.InfoLog("end...");
        }
    }
}
