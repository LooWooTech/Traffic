using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace LoowooTech.Traffic.Common
{
    public static class ExcelHelper
    {
        public static bool SaveExcel(DataTable DataTable, string FilePath,Dictionary<string,int> HeadDict)
        {
            var ext = System.IO.Path.GetExtension(FilePath);
            IWorkbook workbook = null;
            switch (ext)
            {
                case ".xls":
                    workbook = new HSSFWorkbook();
                    break;
                case ".xlsx":
                    workbook = new XSSFWorkbook();
                    break;
            }
            ISheet sheet = workbook.CreateSheet("Sheet1");
            IRow row=sheet.CreateRow(0);
            int Serial = 0;
            foreach (var key in HeadDict.Keys)
            {
                row.CreateCell(Serial++).SetCellValue(key);
            }
            var columnCount = DataTable.Columns.Count;
            var RowCount = DataTable.Rows.Count;
            for (var i = 0; i < RowCount; i++)
            {
                row = sheet.CreateRow(i+1);
                for (var j = 0; j < columnCount; j++)
                {
                    row.CreateCell(j).SetCellValue(DataTable.Rows[i][j].ToString());
                }
            }
            using (var fs = File.OpenWrite(FilePath))
            {
                workbook.Write(fs);
            }
            return true;
        }

        public static IWorkbook OpenWorkbook(string FilePath)
        {
            using (var fs = new FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                return WorkbookFactory.Create(fs);
            }  
        }
    }
}
