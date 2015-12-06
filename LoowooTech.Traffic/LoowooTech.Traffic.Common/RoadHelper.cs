using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LoowooTech.Traffic.Common
{

    public class RoadHelper
    {
        public static Dictionary<int, List<IPoint>> QueryIntersectPoints(IPolyline line, IFeatureClass roadFC)
        {
            var cursor = roadFC.Search(new SpatialFilterClass { Geometry = line, SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects }, true);
            var topo = line as ITopologicalOperator;
            var rel = line as IRelationalOperator;
            var f = cursor.NextFeature();
            var dict = new Dictionary<int, List<IPoint>>();
            while(f!= null)
            {
                var shp = f.ShapeCopy;
                
                var ret = topo.Intersect(f.ShapeCopy, esriGeometryDimension.esriGeometry1Dimension);

                if (ret != null || ret.IsEmpty == false)
                {
                    dict.Add(f.OID, null);
                }
                else
                {
                    ret = topo.Intersect(f.ShapeCopy, esriGeometryDimension.esriGeometry0Dimension);
                    if(ret is IPoint)
                    {
                        var pt = ret as IPoint;
                        dict.Add(f.OID, new List<IPoint>(new [] { pt }));
                    }
                    else if(ret is IPointCollection)
                    {
                        var pc = ret as IPointCollection;
                        var list = new List<IPoint>();
                        for(var i=0;i<pc.PointCount;i++)
                        {
                            list.Add(pc.Point[i]);
                        }
                        dict.Add(f.OID, list);
                    }
                    else
                    {
                        throw new NotSupportedException(string.Format("道路相交结果的类型‘{0}’不被支持", ret.GetType()));
                    }
                }
                f = cursor.NextFeature();
            }

            Marshal.ReleaseComObject(cursor);
            return dict;
        }

        public static List<IPolyline> SplitPolylineInner(IPolyline srcLine, List<IPoint> pts)
        {
            var lines = new List<IPolyline>(new[] { srcLine });
            for (var i = 0; i < pts.Count; i++)
            {
                var pt = pts[i];

                for (var j = 0; j > lines.Count; j++)
                {
                    var line = lines[j];
                    var needSplit = false;
                    var index1 = 0;
                    var index2 = 0;
                    line.SplitAtPoint(pt, false, true, out needSplit, out index1, out index2);
                    if (needSplit)
                    {
                        lines.RemoveAt(j);
                        var gc = line as IGeometryCollection;

                        for (var k = 0; k < gc.GeometryCount; k++)
                        {
                            var g = gc.Geometry[k];
                            if (g is IPolyline)
                            {
                                lines.Add(g as IPolyline);
                            }
                            else
                            {
                                throw new NotSupportedException(string.Format("分割道路的结果类型'{0}'不被支持", g.GetType()));
                            }
                        }
                        break;
                    }
                }
            }
            return lines;
        }

        public static List<int> SplitPolyline(IPolyline srcLine, List<IPoint> pts, Dictionary<string, string> values, IFeatureClass fc, bool dropHead, bool dropTail)
        {
            var lines = SplitPolylineInner(srcLine, pts);
            var cursor = fc.Insert(true);
            var idIndex = cursor.FindField(IDField);
            var count = 0;
            var ret = new List<int>();
            foreach (var line in lines)
            {
                if (dropHead == true && count == 0) continue;
                if (dropTail == true && count == lines.Count - 1) continue;
                var buff = fc.CreateFeatureBuffer();
                var id = GetNewId(fc);
                ret.Add(id);
                buff.set_Value(idIndex, id);
                buff.Shape = line;
                CopyValues(buff, values);
                cursor.InsertFeature(buff);
                cursor.Flush();
            }

            Marshal.ReleaseComObject(cursor);
            return ret;
        }


        public static List<int> SplitPolyline(int oid, List<IPoint> pts, IFeatureClass fc)
        {
            var f = fc.GetFeature(oid);
            
            var geo = f.ShapeCopy as IPolyline;
            var ret = new List<int>();

            var lines = SplitPolylineInner(geo, pts);

            if (lines.Count < 2) return ret ;

            var cursor = fc.Insert(true);
            var idIndex = cursor.FindField(IDField);
            foreach(var line in lines)
            {
                var buff = fc.CreateFeatureBuffer();
                var id = GetNewId(fc);
                ret.Add(id);
                buff.set_Value(idIndex, id);
                buff.Shape = line;
                CopyFields(f, buff);
                cursor.InsertFeature(buff);
                cursor.Flush();
            }

            Marshal.ReleaseComObject(cursor);

            f.Delete();
            return ret;
        }

        private static int GetNewId(IFeatureClass fc)
        {
            var sort = new TableSortClass();
            sort.Ascending[IDField] = false;
            sort.Fields = IDField;
            sort.Table = (ITable)fc;
            sort.Sort(null);
            var cursor = sort.Rows;
            var f = cursor.NextRow();
            if (f == null) return 1;

            var idx = cursor.FindField(IDField);
            return Convert.ToInt32(f.Value[idx]) + 1;
        }


        private static readonly string[] ReservedFields = new string[] { "OBJECTID", "FID", "SHAPE", "SHAPE_LENGTH", "SHAPE_AREA", "NO_" };

        private static readonly string IDField = "NO_";

        private static void CopyFields(IFeature from, IFeatureBuffer to)
        {
            for (var i = 0; i < to.Fields.FieldCount; i++)
            {
                var fld = from.Fields.Field[i];
                var fldName = fld.Name.ToUpper();
                if (ReservedFields.Contains(fldName) == false)
                {
                    var idx = from.Fields.FindField(fld.Name);
                    if (idx > -1)
                    {
                        to.set_Value(i, from.get_Value(idx));
                    }
                }
            }
            
        }

        private static void CopyValues(IFeatureBuffer buff, Dictionary<string, string> values)
        {
            for (var i = 0; i < buff.Fields.FieldCount; i++)
            {
                var fld = buff.Fields.Field[i];
                if(values.ContainsKey(fld.Name) == false) continue;
                var o = values[fld.Name];
                switch(fld.Type)
                {
                    case esriFieldType.esriFieldTypeDate:
                        buff.set_Value(i, Convert.ToDateTime(o));
                        break;
                    case esriFieldType.esriFieldTypeDouble:
                        buff.set_Value(i, Convert.ToDouble(o));
                        break;
                    case esriFieldType.esriFieldTypeInteger:
                        buff.set_Value(i, Convert.ToInt32(o));
                        break;
                    case esriFieldType.esriFieldTypeSingle:
                        buff.set_Value(i, Convert.ToSingle(o));
                        break;
                    case esriFieldType.esriFieldTypeString:
                        buff.set_Value(i, o.ToString());
                        break;
                    default:
                        throw new NotSupportedException(string.Format("不支持赋值类型'{0}'到字段", fld.Type));
                }
            }
        }
    }
}



