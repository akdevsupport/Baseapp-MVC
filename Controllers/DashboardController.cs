using Baseapp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Baseapp.Controllers
{
    public class DashboardController : BaseController
    {
        public ActionResult Index()
        {
            return View(new DashboardModel());
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase ExcelFile)
        {
            var model = new DashboardModel();

            if (ExcelFile != null)
            {
                string path = Server.MapPath("~/Uploaded/" + ExcelFile.FileName);
                ExcelFile.SaveAs(path);
                model.ExcelPath = ExcelFile.FileName;
                // TODO: Excel reading here
            }

            return View(model);
        }

    }
}
