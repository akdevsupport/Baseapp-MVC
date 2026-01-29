using Baseapp.Helper;
using Baseapp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Baseapp.Controllers
{
    public class AccountController : BaseController
    {
        public static ConnectedClient TcpClient = new ConnectedClient();

        public ActionResult Login()
        {
            return View(new LoginModel());
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            string uwd = ConfigurationManager.AppSettings["Uwd"];
            string[] users = uwd.Split('|');
            Models.User user = null;

            foreach (var u in users)
            {
                var parts = u.Split('$');
                if (model.LoginID == parts[0] && model.Password == parts[1])
                {
                    user = new Models.User();
                    user.LoginID = parts[0];
                    user.Role = parts[2];

                    // Connect to saved TCP settings
                    var settings = PrinterSettingsStore.Load();
                    //PrinterService.Initialize(settings.IP, settings.Port);
                    // bool connected = TcpClient.Connect(settings.IP, settings.Port);
                    if (!PrinterManager.Current.Connected)
                    {
                        bool connected = PrinterManager.Current.Connect(settings.IP, settings.Port);
                        if (connected)
                            TempData["msg"] = "TCP Connected Successfully!";
                        else
                            TempData["msg"] = "TCP Connection Failed!";
                    }



                    //if (!string.IsNullOrEmpty(SettingsController.SavedSettings.IP))
                    //{
                    //    bool connected = TcpClient.Connect(
                    //        SettingsController.SavedSettings.IP,
                    //        SettingsController.SavedSettings.Port
                    //    );

                    //    if (connected)
                    //        TempData["msg"] = "TCP Connected Successfully!";
                    //    else
                    //        TempData["msg"] = "TCP Connection Failed!";
                    //}

                    break;
                }
            }

            if (user != null)
            {
                Session["UserName"] = user.LoginID;
                Session["UserRole"] = user.Role;
                return RedirectToAction("Index", "Dashboard");
            }

            model.Message = "Invalid username or password.";
            return View(model);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
