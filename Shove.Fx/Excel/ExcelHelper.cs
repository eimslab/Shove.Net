using System;
using System.Collections.Generic;
using System.Text;
using InteropExcel = Microsoft.Office.Interop.Excel;
using System.Diagnostics;

namespace Shove
{
    /// <summary>
    /// Excel 相关
    /// </summary>
    public partial class _Excel
    {
        /// <summary>
        /// 对 Excel 进行操作的类
        /// </summary>
        public class ExcelHelper
        {
            private InteropExcel._Application ExcelApp;
            private InteropExcel.WorkbookClass wbclass;

            /// <summary>
            /// 构造
            /// </summary>
            /// <param name="FileName"></param>
            public ExcelHelper(string FileName)
            {
                ExcelApp = new InteropExcel.Application();
                object objOpt = System.Reflection.Missing.Value;

                wbclass = (InteropExcel.WorkbookClass)ExcelApp.Workbooks.Open(FileName, objOpt, false, objOpt, objOpt, objOpt, true, objOpt, objOpt, true, objOpt, objOpt, objOpt, objOpt, objOpt);
            }

            /// <summary>
            /// 所有sheet的名称列表
            /// </summary>
            /// <returns></returns>
            public IList<string> GetSheetNames()
            {
                IList<string> result = new List<string>();
                InteropExcel.Sheets sheets = wbclass.Worksheets;

                foreach (InteropExcel.Worksheet sheet in sheets)
                {
                    result.Add(sheet.Name);
                }

                return result;
            }

            /// <summary>
            /// 根据名称获取 sheet
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
            public InteropExcel.Worksheet GetWorksheetByName(string name)
            {
                InteropExcel.Worksheet sheet = null;
                InteropExcel.Sheets sheets = wbclass.Worksheets;

                foreach (InteropExcel.Worksheet ws in sheets)
                {
                    if (ws.Name == name)
                    {
                        sheet = ws;

                        break;
                    }
                }

                return sheet;
            }

            /// <summary>
            /// 获取 sheer 内容
            /// </summary>
            /// <param name="SheetName"></param>
            /// <param name="StartCell">开始单元格</param>
            /// <param name="EndCell">结束单元格</param>
            /// <returns></returns>
            public Array Read(string SheetName, string StartCell, string EndCell)
            {
                InteropExcel.Worksheet sheet = GetWorksheetByName(SheetName);
                InteropExcel.Range rang = sheet.get_Range(StartCell, EndCell);
                System.Array result = (Array)rang.Cells.Value2;

                return result;
            }

            /// <summary>
            /// 释放
            /// </summary>
            public void Close()
            {
                if (ExcelApp != null)
                {
                    ExcelApp.Quit();
                    ExcelApp = null;
                }
            }

            /// <summary>
            /// 析构
            /// </summary>
            ~ExcelHelper()
            {
                Close();
            }
        }
    }
}