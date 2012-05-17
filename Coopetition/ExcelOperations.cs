using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Core;
using Microsoft.Office.Interop;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Collections;

namespace Coopetition
{
    public class ExcelManipulation
    {
        private Microsoft.Office.Interop.Excel.Application app = null;
        private Microsoft.Office.Interop.Excel.Workbook workbook = null;
        private Microsoft.Office.Interop.Excel.Worksheet worksheet = null;
        private Hashtable myHashtable;

        public void CreateExcelFile()
        {
            app = new Microsoft.Office.Interop.Excel.Application();
            workbook = app.Workbooks.Add(1);
            worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Sheets[1];
            AddExcelProcessTolist();
        }

        private void AddExcelProcessTolist()
        {
            Process[] AllProcesses = Process.GetProcessesByName("excel");
            myHashtable = new Hashtable();
            int iCount = 0;

            foreach (Process ExcelProcess in AllProcesses)
            {
                myHashtable.Add(ExcelProcess.Id, iCount);
                iCount = iCount + 1;
            }
        }

        public void createHeaders(int row, int col, string htext, string cell1, string cell2, int mergeColumns, bool font, int size, string fcolor)
        {
            worksheet.Cells[row, col] = htext;
        }

        public void InsertData(int row, int col, string data, string cell1, string cell2, string format)
        {
            worksheet.Cells[row, col] = data;
        }

        public void SaveDocument(string filename)
        { 
            string fileaddress = Constants.BaseDirectory + filename;
            if (!Directory.Exists(Constants.BaseDirectory))
            {
                Directory.CreateDirectory(Constants.BaseDirectory);
            }
            worksheet.Columns.AutoFit();
            workbook.SaveAs(fileaddress, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, null, null,
            false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlShared,
            null, null, null, null, null);
            
             KillExcelProcess();
        }

        private void KillExcelProcess()
        {
            Process[] AllProcesses = Process.GetProcessesByName("excel");

            // check to kill the right process
            foreach (Process ExcelProcess in AllProcesses)
            {
                if (myHashtable.ContainsKey(ExcelProcess.Id))
                    ExcelProcess.Kill();
            }

            // AllProcesses = null;
        }

        public string GetValue(string filename, int row, int col)
        {
            string fileaddress = Constants.BaseDirectory + filename;
            //app.Workbooks.Open(fileaddress, 0, false, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, "", "", false, 
            //                    Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "", true, false, 0, false, false, false);
            workbook = app.Workbooks.Open(fileaddress, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                                Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);
            worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets[1];
            string result = (string)worksheet.Columns[row, col];
            return result;
        }
    }
}
