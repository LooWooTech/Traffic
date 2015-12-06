using ESRI.ArcGIS.esriSystem;
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
        private static bool IsStartOrEnd(IPoint pt, IPointCollection pc)
        {
            var pt2 = pc.get_Point(pc.PointCount - 1);
            var pt1 = pc.get_Point(0);

            if (Math.Abs(pt.X - pt1.X) < 1e-8 && Math.Abs(pt.Y - pt1.Y) < 1e-8) return true;
            if (Math.Abs(pt.X - pt2.X) < 1e-8 && Math.Abs(pt.Y - pt2.Y) < 1e-8) return true;
            return false;
        }

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
                line.SpatialReference = shp.SpatialReference;
                var pc2 = shp as IPointCollection;
                var ret = topo.Intersect(shp, esriGeometryDimension.esriGeometry1Dimension);

                if (ret != null && ret.IsEmpty == false)
                {
                    dict.Add(f.OID, null);
                }
                else
                {
                    ret = topo.Intersect(shp, esriGeometryDimension.esriGeometry0Dimension);
                    if(ret is IPoint)
                    {
                        var pt = ret as IPoint;
                        if (IsStartOrEnd(pt, pc2) == false)
                        {
                            dict.Add(f.OID, new List<IPoint>(new[] { pt }));
                        }
                    }
                    else if(ret is IPointCollection)
                    {
                        var pc = ret as IPointCollection;
                        var list = new List<IPoint>();
                        for (var i = 0; i < pc.PointCount; i++)
                        {
                            if (IsStartOrEnd(pc.Point[i], pc2) == false)
                            {
                                list.Add(pc.Point[i]);
                            }
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

        private static IPolyline CopyPolyline(IPolyline line)
        {
            var objCopy = new ObjectCopyClass();
            var copy = objCopy.Copy(line) as IPolyline;
            return copy;
        }


        public static List<IPolyline> SplitPolylineInner(IPolyline srcLine, List<IPoint> pts)
        {
            srcLine = CopyPolyline(srcLine);
            var lines = new List<IPolyline>(new[] { srcLine });
            for (var i = 0; i < pts.Count; i++)
            {
                var pt = pts[i];

                for (var j = 0; j < lines.Count; j++)
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
                                lines.Insert(j, g as IPolyline);
                            }
                            else if(g is IPath)
                            {
                                var pl = new PolylineClass();
                                var gc2 = pl as IGeometryCollection;
                                gc2.AddGeometry(g as IPath);
                                lines.Insert(j, pl);
                            }
                            else
                            {
                                throw new NotSupportedException(string.Format("分割道路的结果类型'{0}'不被支持", g.GeometryType));
                            }
                        }

                        break;
                    }
                }
            }
            return lines;
        }

        private static void GetDistrictInfo(IPolyline line, IFeatureClass fc, out string districtName, out string districtNO)
        {
            var cursor = fc.Search(new SpatialFilterClass { Geometry = line, SpatialRel = esriSpatialRelEnum.esriSpatialRelWithin }, true);
            var f = cursor.NextFeature();
            if(f!= null)
            {
                districtName = f.get_Value(f.Fields.FindField("NAME")).ToString();
                districtNO = f.get_Value(f.Fields.FindField("NO_")).ToString();
            }
            else
            {
                districtName = string.Empty;
                districtNO = string.Empty;
            }
            Marshal.ReleaseComObject(cursor);
        }

        public static List<int> SplitPolyline(IPolyline srcLine, List<IPoint> pts, Dictionary<string, string> values, IFeatureClass fc, IFeatureClass districtFC, bool dropHead, bool dropTail)
        {
            var lines = SplitPolylineInner(srcLine, pts);
            var cursor = fc.Insert(true);
            var idIndex = cursor.FindField(IDField);
            var count = 0;
            var ret = new List<int>();
            var id = GetNewId(fc);
            var dIndex = cursor.FindField("DISTRICT");
            var dIndex2 = cursor.FindField("DISTRICTNO");
            foreach (var line in lines)
            {
                if ((dropHead == true && count == 0) || (dropTail == true && count == lines.Count - 1)) 
                {
                    count++;
                    continue;
                }
                
                var buff = fc.CreateFeatureBuffer();
                buff.Shape = line;
                CopyValues(buff, values);
                string d,dNO;
                GetDistrictInfo(line, districtFC, out d, out dNO);
                buff.set_Value(dIndex, d);
                if(string.IsNullOrEmpty(dNO))
                {
                    buff.set_Value(dIndex2, DBNull.Value);
                }
                else
                {
                    buff.set_Value(dIndex2, int.Parse(dNO));
                }
                
                buff.set_Value(idIndex, id);
                ret.Add(id);
                cursor.InsertFeature(buff);
                cursor.Flush();
                id++;
                count++;
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
            var id = GetNewId(fc);
            foreach(var line in lines)
            {
                var buff = fc.CreateFeatureBuffer();
                ret.Add(id);
                buff.set_Value(idIndex, id);
                buff.Shape = line;
                CopyFields(f, buff);
                cursor.InsertFeature(buff);
                cursor.Flush();
                id++;
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


        private static readonly string[] ReservedFields = new string[] { "OBJECTID", "FID", "SHAPE", "SHAPE_LENGTH", "SHAPE.LEN", "SHAPE_AREA", "NO_" };

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
                if (string.IsNullOrEmpty(o))
                {
                    if(fld.Type == esriFieldType.esriFieldTypeString)
                    {
                        buff.set_Value(i, o);
                    }
                    else
                    {
                        buff.set_Value(i, DBNull.Value);
                    }
                }
                else
                {
                    switch (fld.Type)
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
}



