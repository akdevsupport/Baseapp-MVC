using Baseapp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing.Printing;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Wordprocessing;
using Baseapp.Helper;

namespace Baseapp.Controllers
{
    public class SettingsController : BaseController
    {
        public ActionResult Index()
        {
            var settings = PrinterSettingsStore.Load();
            return View(settings);
        }

        [HttpPost]
        public ActionResult Index(ConnectionSettings model)
        {
            PrinterSettingsStore.Save(model);
            TempData["msg"] = "Settings saved!";
            return View(model);
        }
    }
}
