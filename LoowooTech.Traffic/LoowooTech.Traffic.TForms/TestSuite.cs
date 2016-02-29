using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using LoowooTech.Traffic.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace LoowooTech.Traffic.TForms
{
    public static class TestSuite
    {
        // 测试道路历史
        public static Form TestCase2()
        {
            var factory = new AccessWorkspaceFactory();
            var ws = factory.OpenFromFile(@"C:\Temp\Traffic\db.mdb", 0) as IFeatureWorkspace;
            var fc = ws.OpenFeatureClass("AROAD");
            var historyFC = ws.OpenFeatureClass("ROADHIS");
            var nodeFC = ws.OpenFeatureClass("NODE");

            var form = new HistoryForm(8740, fc, historyFC, nodeFC, null);
            return form;
        }

        // 测试导入道路
        public static Form TestCase1()
        {
           

            var factory2 = new CadWorkspaceFactory();
            try
            {
                var path = System.IO.Path.GetDirectoryName(@"C:\Temp\Traffic\temp.dxf");
                var ws2 = factory2.OpenFromFile(path, 0) as IFeatureWorkspace;

                var fc2 = ws2.OpenFeatureClass(System.IO.Path.GetFileName(@"C:\Temp\Traffic\temp.dxf") + ":polyline");

                var cursor = fc2.Search(null, true);
                var f = cursor.NextFeature();

                var lst = new List<IPolyline>();
                while (f != null)
                {
                    var geo = f.ShapeCopy;
                    if (!(geo is IPolyline) || geo.IsEmpty == true)
                    {
                        MessageBox.Show("当前CAD文件中包含的路线类型信息不正确，请检查。", "注意");
                        Marshal.ReleaseComObject(cursor);
                        return null;
                    }
                    else
                    {
                        lst.Add(geo as IPolyline);
                    }
                    f = cursor.NextFeature();
                }
                Marshal.ReleaseComObject(cursor);


                if (lst.Count == 0)
                {
                    MessageBox.Show("当前CAD文件中不包含路线信息，请检查。", "注意");
                    return null;
                }


                RoadMerger.FragmentThreshold = 20;
                var ls = RoadMerger.SplitLine(lst);

                var factory = new AccessWorkspaceFactory();
                var ws = factory.OpenFromFile(@"C:\Temp\Traffic\db.mdb", 0) as IFeatureWorkspace;
                var fc = ws.OpenFeatureClass("AROAD");
                var historyFC = ws.OpenFeatureClass("ROADHIS");
                var nodeFC = ws.OpenFeatureClass("NODE");

                var lines = RoadMerger.QueryIntersects(ls, fc);

                var form = new ImportRoadForm(ls, fc, historyFC, nodeFC, lines);
                return form;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "注意");
                return null;
            }
        }
        
    }
}
