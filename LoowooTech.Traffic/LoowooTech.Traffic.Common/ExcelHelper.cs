using ESRI.ArcGIS.Geodatabase;
using LoowooTech.Traffic.Models;
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
        public static bool SaveExcel(DataTable DataTable, string FilePath)
        {
            IWorkbook workbook = CreateWorkBook(FilePath);
            ISheet sheet = workbook.CreateSheet("Sheet1");
            NPOI.SS.UserModel.IRow row=sheet.CreateRow(0);
            int Serial = 0;
            foreach (var item in DataTable.Columns)
            {
                row.CreateCell(Serial++).SetCellValue(item.ToString());
            }
            //foreach (var key in HeadDict.Keys)
            //{
            //    row.CreateCell(Serial++).SetCellValue(key);
            //}
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
            Save(workbook, FilePath);
            return true;
        }
        private static int GetIndex(DataTable dataTable,string fieldName)
        {
            var index = 0;
            for (var i = 0; i < dataTable.Columns.Count; i++)
            {
                if (dataTable.Columns[i].ToString() == fieldName)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }
        public static Dictionary<string, double> Statistic(DataTable DataTable,string FieldName)
        {
            int Index = GetIndex(DataTable, FieldName);
            string val = string.Empty;
            var dict = new Dictionary<string, double>();
            for (var i = 0; i < DataTable.Rows.Count; i++)
            {
                val = DataTable.Rows[i][Index].ToString();
                if (!string.IsNullOrEmpty(val))
                {
                    if (dict.ContainsKey(val))
                    {
                        dict[val] = dict[val] + 1;
                    }
                    else
                    {
                        dict.Add(val, 1);
                    }
                }
            }
            return dict;
        }
        public static Dictionary<string,double> Statistic2(DataTable dataTable,string labelName,string statisticName)
        {
            var labelIndex = GetIndex(dataTable, labelName);
            var statisticIndex = GetIndex(dataTable, statisticName);
            
            var dict = new Dictionary<string, double>();
            var key = "";
            var val = .0;
            for (var i = 0; i < dataTable.Rows.Count; i++)
            {
                key = dataTable.Rows[i][labelIndex].ToString();
                if(double.TryParse(dataTable.Rows[i][statisticIndex].ToString(),out val))
                {
                    if (dict.ContainsKey(key))
                    {
                        dict[key] += val;
                    }
                    else
                    {
                        dict.Add(key, val);
                    }
                }
               
            }
            return dict;
        }

        public static int Statistic2(DataTable DataTable, string FieldName)
        {
            int Index = 0;
            for (var i = 0; i < DataTable.Columns.Count; i++)
            {
                if (DataTable.Columns[i].ToString() == FieldName)
                {
                    Index = i;
                    break;
                }
            }
            int Sum = 0;
            int temp = 0;
            for (var i = 0; i < DataTable.Rows.Count; i++)
            {
                if (int.TryParse(DataTable.Rows[i][Index].ToString(), out temp))
                {
                    Sum += temp;
                }
            }
            return Sum;
        }
        public static void SaveExcel(IFeatureClass FeatureClass, string WhereClause, string FilePath, Dictionary<string, int> HeadDict) 
        {
            IWorkbook workbook = CreateWorkBook(FilePath);
            ISheet sheet = workbook.CreateSheet("Sheet1");
            NPOI.SS.UserModel.IRow row = sheet.CreateRow(0);
            int serial = 0;
            var dict = LayerInfoHelper.GetLayerDictionary(FeatureClass.AliasName.GetAlongName());
            foreach (var key in HeadDict.Keys)
            {
                if (dict.ContainsKey(key))
                {
                    row.CreateCell(serial++).SetCellValue(dict[key]);
                }
                else
                {
                    row.CreateCell(serial++).SetCellValue(key);
                }
                
            }
            int Index = 1;
            IQueryFilter queryFilter = new QueryFilterClass();
            queryFilter.WhereClause = WhereClause;
            IFeatureCursor featureCursor = FeatureClass.Search(queryFilter, false);
            IFeature feature = featureCursor.NextFeature();
            while (feature != null)
            {
                row = sheet.CreateRow(Index++);
                serial=0;
                foreach (var val in HeadDict.Values)
                {
                    row.CreateCell(serial++).SetCellValue(feature.get_Value(val).ToString());
                }
                feature = featureCursor.NextFeature();
            }
            Save(workbook, FilePath);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(featureCursor);
        }
        private static void Save(IWorkbook workbook, string FilePath)
        {
            using (var fs = File.OpenWrite(FilePath))
            {
                workbook.Write(fs);
            }
        }
        public static IWorkbook CreateWorkBook(string FilePath)
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
            return workbook;
        }
        public static IWorkbook OpenWorkbook(string FilePath)
        {
            using (var fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
            {
                return WorkbookFactory.Create(fs);
            }  
        }

        private static string GetCellValue(this ICell Cell)
        {
            if (Cell != null)
            {
                switch (Cell.CellType)
                {
                    case CellType.String:
                        return Cell.StringCellValue;
                    case CellType.Numeric:
                        return Cell.NumericCellValue.ToString();
                    case CellType.Formula:
                        double val = 0.0;
                        try
                        {
                            val = double.Parse(Cell.NumericCellValue.ToString());
                        }
                        catch
                        {

                        }
                        return val.ToString();
                    default:
                        return Cell.ToString();

                }
            }
            return string.Empty;
           
        }
        public static List<BusLine> Read(string FilePath)
        {
            IWorkbook workbook = OpenWorkbook(FilePath);
            ISheet sheet = workbook.GetSheetAt(0);
            int RowCount = sheet.LastRowNum;
            DirectType directType=DirectType.Up;
            NPOI.SS.UserModel.IRow row = null;
            var list = new List<BusLine>();
            for (var i = 1; i <= RowCount; i++)
            {
                row = sheet.GetRow(i);
                if (row == null)
                {
                    continue;
                }
                switch (int.Parse(row.GetCell(4).GetCellValue()))
                {
                    case 1:
                        directType = DirectType.Up;
                        break;
                    case 2:
                        directType = DirectType.Down;
                        break;
                }
                list.Add(new BusLine()
                {
                    LineName = row.GetCell(2).GetCellValue(),
                    ShortName = row.GetCell(3).GetCellValue(),
                    Direction = directType,
                    StartStop = row.GetCell(6).GetCellValue(),
                    EndStop = row.GetCell(7).GetCellValue()
                });
            }
                
            return list;
        }
    }
}
