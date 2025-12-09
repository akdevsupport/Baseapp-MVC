using DocumentFormat.OpenXml.Spreadsheet;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Baseapp.Helpers
{
    public class FileConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((string)parameter == (string)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value ? parameter : null;
        }
    }
    public class ExcelHelper
    {
        public DataTable ReadData(string filePath, bool header = true)
        {
            DataTable dt = new DataTable();
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    IWorkbook workbook = new HSSFWorkbook(fs);
                    ISheet sheet = workbook.GetSheetAt(0);
                    //Read Header row
                    IRow row = sheet.GetRow(0);
                    if (header)
                    {
                        for (int i = 0; i < row.Cells.Count; i++)
                        {
                            dt.Columns.Add(row.GetCell((i)).ToString().Replace(".", "").Replace(" ", "_"));
                        }
                    }
                    else
                    {
                        for (int i = 0; i < row.Cells.Count; i++)
                        {
                            dt.Columns.Add($"Column{i}");
                        }
                    }
                    int dataRows = sheet.PhysicalNumberOfRows;
                    for (int j = 1; j < dataRows; j++)
                    {
                        IRow r = sheet.GetRow(j);
                        DataRow dr = dt.NewRow();
                        for (int i = 0; i < row.Cells.Count; i++)
                        {
                            if (r.GetCell(i) != null)
                            {
                                dr[dt.Columns[i]] = r.GetCell(i).ToString();
                            }
                            else
                            {
                                dr[dt.Columns[i]] = "";
                            }
                        }
                        dt.Rows.Add(dr);
                    }
                }
            }
            catch (Exception ex)
            {
                
            }

            return dt;
        }
        public void WriteData(DataTable dataTable, string filePath, string Sheetname, bool header = true)
        {
            // Create XLS Workbook
            IWorkbook workbook = new HSSFWorkbook(); // Use XSSFWorkbook() for XLSX
            ISheet sheet = workbook.CreateSheet(Sheetname);

            if (header)
            {
                // Write Header
                IRow headerRow = sheet.CreateRow(0);
                for (int i = 0; i < dataTable.Columns.Count; i++)
                    headerRow.CreateCell(i).SetCellValue(dataTable.Columns[i].ColumnName.Replace(".", "").Replace(" ", "_"));
            }

            // Write Data
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                IRow row = header ? sheet.CreateRow(i + 1) : sheet.CreateRow(i);
                for (int j = 0; j < dataTable.Columns.Count; j++)
                    row.CreateCell(j).SetCellValue(dataTable.Rows[i][j].ToString());
            }
            // Save to File
            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                workbook.Write(fs);
            }
        }
    }
    public class ReportHelper
    {
        //This method convertrs the DataTable to Csv (in the form of StringBuilder instance).
        public static StringBuilder ConvertDataTableToCsvFile(DataTable dtData, bool header = false)
        {
            StringBuilder data = new StringBuilder();
            if (header)
            {
                //Taking the column names.
                for (int column = 0; column < dtData.Columns.Count; column++)
                {
                    //Making sure that end of the line, shoould not have comma delimiter.
                    if (column == dtData.Columns.Count - 1)
                        data.Append(dtData.Columns[column].ColumnName.ToString().Replace(",", ";"));
                    else
                        data.Append(dtData.Columns[column].ColumnName.ToString().Replace(",", ";") + ',');
                }

                data.Append(Environment.NewLine);//New line after appending columns.
            }

            for (int row = 0; row < dtData.Rows.Count; row++)
            {
                for (int column = 0; column < dtData.Columns.Count; column++)
                {
                    ////Making sure that end of the line, shoould not have comma delimiter.
                    if (column == dtData.Columns.Count - 1)
                        data.Append(dtData.Rows[row][column].ToString().Replace(",", ";"));
                    else
                        data.Append(dtData.Rows[row][column].ToString().Replace(",", ";") + ',');
                }

                //Making sure that end of the file, should not have a new line.
                if (row != dtData.Rows.Count - 1)
                    data.Append(Environment.NewLine);
            }
            return data;
        }
        public static string GenerateReport<T>(List<T> items, List<string> ignoreColumns = null) where T : class
        {
            var output = "";
            var delimiter = ",";
            var properties = typeof(T).GetProperties()
             .Where(n =>
             n.PropertyType == typeof(string)
             || n.PropertyType == typeof(bool)
             || n.PropertyType == typeof(char)
             || n.PropertyType == typeof(byte)
             || n.PropertyType == typeof(decimal)
             || n.PropertyType == typeof(int)
             || n.PropertyType == typeof(DateTime)
             || n.PropertyType == typeof(DateTime?));
            if (ignoreColumns == null)
            {
                ignoreColumns = new List<string>();
            }
            //TODO: Get description attribute from PropertyName
            var sw = new StringBuilder();
            //.Select(n => n.Name)
            var header = properties
            .Where(p => !ignoreColumns.Contains(p.Name))
            .Select(n => (Attribute.GetCustomAttribute(n, typeof(DescriptionAttribute)) as DescriptionAttribute).Description)
            .Aggregate((a, b) => a + delimiter + b);
            sw.AppendLine(header);
            foreach (var item in items)
            {
                var row = properties
                .Where(p => !ignoreColumns.Contains(p.Name))
                .Select(n => n.GetValue(item, null))
                .Select(n => n == null ? "null" : "\"" + n.ToString() + "\"").Aggregate((a, b) => a + delimiter + b);
                sw.AppendLine(row);
            }
            output = sw.ToString();
            return output;
        }
    }
}