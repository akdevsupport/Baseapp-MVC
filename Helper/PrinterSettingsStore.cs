using Baseapp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Baseapp.Helper
{
    public static class PrinterSettingsStore
    {
        private static string FilePath =>  HttpContext.Current.Server.MapPath("~/App_Data/printer-settings.json");

        public static ConnectionSettings Load()
        {
            if (!File.Exists(FilePath))
                return new ConnectionSettings();

            return JsonConvert.DeserializeObject<ConnectionSettings>(
                File.ReadAllText(FilePath));
        }

        public static void Save(ConnectionSettings settings)
        {
            File.WriteAllText(
                FilePath,
                JsonConvert.SerializeObject(settings, Formatting.Indented)
            );
        }
    }
}
