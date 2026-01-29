using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Baseapp.Helper
{
    public static class PrinterManager
    {
        public static Printer Current { get; } = new Printer();
    }

}
