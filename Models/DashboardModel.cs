using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baseapp.Models
{
    public class DashboardModel
    {
        public string ExcelPath { get; set; }
        public DataTable Data { get; set; }
    }
}
