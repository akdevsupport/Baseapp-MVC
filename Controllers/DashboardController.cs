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
using ClosedXML.Excel;
using ExcelDataReader;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Data;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using Baseapp.Helpers;
using DocumentFormat.OpenXml.Office2010.CustomUI;
using System.Threading;
using DocumentFormat.OpenXml.EMMA;
using System.Diagnostics;
using System.ComponentModel;
using OfficeOpenXml;

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
            DashboardModel model = new DashboardModel();
            model.Data = new DataTable();

            if (ExcelFile != null && ExcelFile.ContentLength > 0)
            {
                string fileName = Path.GetFileName(ExcelFile.FileName);
                string path = Server.MapPath("~/Uploaded/" + fileName);

                if (!Directory.Exists(Server.MapPath("~/Uploaded")))
                    Directory.CreateDirectory(Server.MapPath("~/Uploaded"));

                ExcelFile.SaveAs(path);
                model.ExcelPath = fileName;

                string ext = Path.GetExtension(fileName).ToLower();

                if (ext == ".csv")
                    model.Data = ReadCSV(path);
                else if (ext == ".xlsx")
                    model.Data = ReadExcel(path);
                else
                    model.Data = null;
            }

            return View(model);
        }

        private DataTable ReadCSV(string filePath)
        {
            DataTable dt = new DataTable();
            string[] lines = System.IO.File.ReadAllLines(filePath);

            if (lines.Length > 0)
            {
                // Columns
                string[] header = lines[0].Split(',');
                foreach (var h in header)
                    dt.Columns.Add(h);

                // Rows
                for (int i = 1; i < lines.Length; i++)
                {
                    var data = lines[i].Split(',');
                    dt.Rows.Add(data);
                }
            }
            return dt;
        }

        private DataTable ReadExcel(string filePath)
        {
            DataTable dt = new DataTable();

            using (var workbook = new ClosedXML.Excel.XLWorkbook(filePath))
            {
                var ws = workbook.Worksheet(1); 

                bool firstRow = true;

                foreach (var row in ws.RowsUsed())
                {
                    if (firstRow)
                    {
                        foreach (var cell in row.Cells())
                        {
                            dt.Columns.Add(cell.GetValue<string>());
                        }
                        firstRow = false;
                    }
                    else
                    {
                        dt.Rows.Add(row.Cells().Select(c => c.GetValue<string>()).ToArray());
                    }
                }
            }
            return dt;
        }
    }
}
