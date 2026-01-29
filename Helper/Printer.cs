using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Baseapp.Helper
{
    public class Printer : ConnectedClient
    {
        public string PrinterStatus { get; private set; } = "DISCONNECTED";
        public int PrintCount { get; private set; }
        public List<KeyValuePair> JobList { get; private set; }

        public Printer()
        {
            OnDataReceived += Printer_OnDataReceived;
        }

        private void Printer_OnDataReceived(object sender, string data)
        {
            string[] responses = Regex.Split(data, "\r");

            foreach (var response in responses)
            {
                if (string.IsNullOrWhiteSpace(response))
                    continue;

                // ===== STS (STATUS) =====
                if (response.ToUpper().Contains("STS"))
                {
                    string[] arResponse = response.Split('|');

                    if (arResponse.Length > 1)
                    {
                        switch (arResponse[1])
                        {
                            case "3":
                                PrinterStatus = "RUNNING";
                                break;
                            case "4":
                                PrinterStatus = "OFFLINE";
                                break;
                            default:
                                PrinterStatus = "OFFLINE";
                                break;
                        }
                    }
                }

                // ===== PRC (PRINT COUNT) =====
                if (response.ToUpper().Contains("PRC"))
                {
                    if (int.TryParse(response.Split('|').Last(), out int count))
                    {
                        PrintCount = count;
                    }
                }

                // ===== JBL (JOB LIST) =====
                //if (response.ToUpper().StartsWith("JBL"))
                //{
                //    string[] JBLResponse = response.Split('|');
                //    if (JBLResponse.Length > 2)
                //    {
                //        List<string> jobs = JBLResponse.Skip(2).ToList();
                //        JobList = new List<KeyValuePair>();
                //        List<KeyValuePair> J = new List<KeyValuePair>();
                //        foreach (var j in jobs)
                //        {
                //            J.Add(new KeyValuePair(j, j, j));
                //        }
                //        JobList = J;
                //    }
                //    // Parse job list if needed later
                //}
            }
        }
        //public void RequestStatus()
        //{
        //    Send("GST" + (char)13);
        //}
    }
}
