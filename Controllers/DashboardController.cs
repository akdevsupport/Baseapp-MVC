using Baseapp.Models;
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
using NPOI.OpenXml4Net.OPC.Internal;
using Baseapp.Helper;
using System.Net.Sockets;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Baseapp.Controllers
{
    public class DashboardController : BaseController
    {
        //   private static ConnectedClient _client;
        //readonly string CR = "\r";
        readonly char CR = (char)13;

        private ConnectedClient _client => AccountController.TcpClient;
        public DashboardController()
        {
            if (!PrinterManager.Current.Connected)
            {
                TryReconnect();
            }
            GetPrinterStatus();
            DoGetPrinterJob();
        }
        public ActionResult Index()
        {
            GetPrinterStatus();
            ViewBag.PrinterStatus = PrinterManager.Current.PrinterStatus;
            ViewBag.PrintCount = PrinterManager.Current.PrintCount;

            ViewBag.JobList = PrinterManager.Current.JobList != null
                ? new SelectList(
                    PrinterManager.Current.JobList,
                    "Value",
                    "Label"
                  ) : new SelectList(new List<string>());
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

        public ActionResult PrinterStatus()
        {
            return Json(new
            {
                status = PrinterManager.Current.PrinterStatus,
                count = PrinterManager.Current.PrintCount
            }, JsonRequestBehavior.AllowGet);
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
        private static bool _connecting;
        private static void TryReconnect()
        {
            _connecting = true;

            new Thread(() =>
            {
                int attempts = 0;
                int maxAttempts = 5;

                while (attempts < maxAttempts)
                {
                    try
                    {
                        var settings = PrinterSettingsStore.Load();
                        bool connected = AccountController.TcpClient
                        .Connect(settings.IP, settings.Port);

                        if (AccountController.TcpClient.Connected)
                            break;
                    }
                    catch
                    {
                        attempts++;
                        Thread.Sleep(2000);
                    }
                }
                _connecting = false;
            })
            { IsBackground = true }.Start();
        }
        public void GetPrinterStatus()
        {
            try
            {
                if (PrinterManager.Current.Connected)
                {
                    PrinterManager.Current.Send("GST" + CR);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error on the connection to the Printer{ex}");
            }
        }
        public void DoGetPrinterJob()
        {
            try
            {
                if (PrinterManager.Current.Connected)
                {
                    PrinterManager.Current.Send("GJL" + CR);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error on the connection to the Printer{ex}");
            }
        }
        public ActionResult DoPrinterOnline()
        {
            string command = "SST|3|" + CR;
            if (PrinterManager.Current.Connected)
            {
               PrinterManager.Current.Send(command);
            }
            Thread.Sleep(500);
            GetPrinterStatus();


            return View("Index", new DashboardModel());
        }
        public ActionResult DoPrinterOffline()
        {
            string command = "SST|4|" + CR;
            if (PrinterManager.Current.Connected)
            {
                PrinterManager.Current.Send(command);
            }
            Thread.Sleep(500);
            GetPrinterStatus();

            return View("Index", new DashboardModel());
        }
    }
}
