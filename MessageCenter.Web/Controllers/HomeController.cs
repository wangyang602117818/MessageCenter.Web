using MessageCenter.Business;
using SSO.Util.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace MessageCenter.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return Content("index");
        }
    }
}