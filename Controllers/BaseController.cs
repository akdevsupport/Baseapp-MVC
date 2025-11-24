using Baseapp.Models;
using System.Web.Mvc;
using System.Web;

namespace Baseapp.Controllers
{
    public class BaseController : Controller
    {
        protected BootstrapModel UI = new BootstrapModel();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["UserName"] != null)
            {
                UI.UserName = Session["UserName"].ToString();
                UI.UserRole = int.Parse(Session["UserRole"].ToString());
                UI.LoginVisible = false;
                UI.LogoutVisible = true;
                UI.DashboardVisible = true;
                UI.SettingsVisible = (UI.UserRole == 1);
                UI.ReportVisible = (UI.UserRole == 1);
            }

            ViewBag.UI = UI;
            base.OnActionExecuting(filterContext);
        }
    }
}
