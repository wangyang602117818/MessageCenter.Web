using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MessageCenter.Web.Controllers
{
    public class FileTaskQueueController : Controller
    {
        // GET: FileTaskQueue
        public ActionResult Index()
        {
            return View();
        }
    }
}