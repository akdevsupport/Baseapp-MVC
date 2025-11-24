using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baseapp.Models
{
    public class BootstrapModel
    {
        public bool LoginVisible { get; set; } = true;
        public bool DashboardVisible { get; set; }
        public bool SettingsVisible { get; set; }
        public bool ReportVisible { get; set; }
        public bool LogoutVisible { get; set; }
        public string UserName { get; set; }
        public int UserRole { get; set; }
    }

}
