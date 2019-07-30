using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;
using System.Diagnostics;

namespace Shove
{
    /// <summary>
    /// Excel 相关
    /// </summary>
    public partial class _Excel
    {
        /// <summary>
        /// 将数据表 DataTable 转换为 Excel 工作簿
        /// </summary>
        /// <param name="dt">数据表</param>
        public static Microsoft.Office.Interop.Excel.Workbook DataTableToWorkBook(System.Data.DataTable dt)
        {
            if (dt.Columns.Count < 1)
            {
                throw new Exception("_Excel.DataTableToWorkBook 方法提供的 DataTable 参数找不到任何可用的列。");
            }

            string[] Cells = new string[dt.Columns.Count];

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                Cells[i] = dt.Columns[i].ColumnName;
            }

            return DataTableToWorkBook(dt, Cells);
        }

        /// <summary>
        /// 将数据表 DataTable 转换为 Excel 工作簿
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="Cells">指定字段名</param>
        public static Microsoft.Office.Interop.Excel.Workbook DataTableToWorkBook(System.Data.DataTable dt, string[] Cells)
        {
            if (dt.Columns.Count < 1)
            {
                throw new Exception("_Excel.DataTableToWorkBook 方法提供的 DataTable 参数找不到任何可用的列。");
            }

            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbooks workbooks = xlApp.Workbooks;
            Microsoft.Office.Interop.Excel.Workbook workbook = workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);
            Microsoft.Office.Interop.Excel.Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets[1];
            Microsoft.Office.Interop.Excel.Range range = null;

            long totalCount = dt.Rows.Count;
            long rowRead = 0;
            float percent = 0;

            // 写入列标题
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                if ((Cells != null) && (Cells.Length > i) && (!String.IsNullOrEmpty(Cells[i])))
                {
                    worksheet.Cells[1, i + 1] = Cells[i].Trim();
                }
                else
                {
                    worksheet.Cells[1, i + 1] = dt.Columns[i].ColumnName;
                }

                range = (Microsoft.Office.Interop.Excel.Range)worksheet.Cells[1, i + 1];    //自动调整行高
            }

            // 写入内容
            for (int r = 0; r < dt.Rows.Count; r++)
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    worksheet.Cells[r + 2, i + 1] = dt.Rows[r][i];
                    range = (Microsoft.Office.Interop.Excel.Range)worksheet.Cells[r + 2, i + 1];
                }
                rowRead++;
                percent = ((float)(100 * rowRead)) / totalCount;
            }

            return workbook;
        }

        /// <summary>
        /// 将数据表 DataTable 转换为 Excel 工作簿并下载，系统根目录下需要建 Temp 目录，并授予访问者读取、写入权限
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="FileName">要保存的文件名</param>
        public static void DataTableToWorkBookAndDownload(System.Data.DataTable dt, string FileName)
        {
            HttpResponse response = HttpContext.Current.Response;
            response.ContentEncoding = System.Text.Encoding.Default;

            if (String.IsNullOrEmpty(FileName))
            {
                FileName = "Excel.xls";
            }

            Microsoft.Office.Interop.Excel.Workbook wb = DataTableToWorkBook(dt);
            string TempFileName = WriteWorkBookToTempFile(wb);

            response.AppendHeader("Content-Disposition", "attachment;filename=" + FileName);
            response.WriteFile(TempFileName); // 将 DataTable 转换成 Excel 工作簿，并下载
            response.End();
        }

        /// <summary>
        /// 将数据表 DataTable 转换为 Excel 工作簿并下载，系统根目录下需要建 Temp 目录，并授予访问者读取、写入权限
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="FileName">要保存的文件名</param>
        /// <param name="Cells">指定字段名</param>
        public static void DataTableToWorkBookAndDownload(System.Data.DataTable dt, string FileName, string[] Cells)
        {
            HttpResponse response = HttpContext.Current.Response;

            response.ContentEncoding = System.Text.Encoding.Default;

            if (String.IsNullOrEmpty(FileName))
            {
                FileName = "Excel.xls";
            }

            Microsoft.Office.Interop.Excel.Workbook wb = DataTableToWorkBook(dt, Cells);
            string TempFileName = WriteWorkBookToTempFile(wb);

            response.AppendHeader("Content-Disposition", "attachment;filename=" + FileName);
            response.WriteFile(TempFileName); // 将 DataTable 转换成 Excel 工作簿，并下载
            response.End();
        }

        private static string WriteWorkBookToTempFile(Microsoft.Office.Interop.Excel.Workbook wb)
        {
            string TempPath = System.AppDomain.CurrentDomain.BaseDirectory + "Temp";

            if (!Directory.Exists(TempPath))
            {
                Directory.CreateDirectory(TempPath);
            }

            string TempFileName = TempPath + "/" + System.Guid.NewGuid().ToString("N") + ".xls";
            wb.SaveAs(TempFileName);

            return TempFileName;
        }

        /// <summary>
        /// 将 DataGrid、DataList 等数据控件中的数据导出为 Execl 文档并下载
        /// </summary>
        /// <param name="ctl">控件</param>
        public static void DataControlToExcelAndDownload(System.Web.UI.Control ctl)
        {
            DataControlToExcelAndDownload(ctl, "Excel.xls");
        }

        /// <summary>
        /// 将 DataGrid、DataList 等数据控件中的数据导出为 Execl 文档并下载
        /// </summary>
        /// <param name="ctl">控件</param>
        /// <param name="FileName">指定保存的文件名</param>
        public static void DataControlToExcelAndDownload(System.Web.UI.Control ctl, string FileName)
        {
            if (String.IsNullOrEmpty(FileName))
            {
                FileName = "Excel.xls";
            }

            HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + FileName);
            HttpContext.Current.Response.Charset = System.Text.Encoding.Default.EncodingName;
            HttpContext.Current.Response.ContentEncoding = System.Text.Encoding.Default;
            HttpContext.Current.Response.ContentType = "application/ms-excel";

            ctl.Page.EnableViewState = false;
            System.IO.StringWriter sw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter htw = new System.Web.UI.HtmlTextWriter(sw);
            ctl.RenderControl(htw);

            HttpContext.Current.Response.Write(sw.ToString());
            HttpContext.Current.Response.End();
        }

        /// <summary>
        /// 强行杀死最近打开的 Excel 进程
        /// </summary>
        private static void KillLastExcelProcesses()
        {
            System.Diagnostics.Process[] excelProc = System.Diagnostics.Process.GetProcessesByName("EXCEL");
            System.DateTime startTime = new DateTime();
            int m, killId = 0;

            for (m = 0; m < excelProc.Length; m++)
            {
                if (startTime < excelProc[m].StartTime)
                {
                    startTime = excelProc[m].StartTime;
                    killId = m;
                }
            }

            if (excelProc[killId].HasExited == false)
            {
                excelProc[killId].Kill();
            }
        }
    }
}
