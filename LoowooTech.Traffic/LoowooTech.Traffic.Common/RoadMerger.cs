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
    /// <summary>
    /// 1. 去除自相交，生成新打断后的导入路线
    /// 2. 查询与已有的路线相交点
    /// 3. 查询出头点的信息
    /// </summary>
    public class RoadMerger
    {
        private static readonly string[] ReservedFields = new string[] { "OBJECTID", "FID", "SHAPE", "SHAPE_LENGTH", "SHAPE.LEN", "SHAPE_AREA", "NO_" };

        public static readonly string IDFieldName = "NO_";

        public static readonly string ParentIDFieldName = "PNO_";

        public static readonly string CreateTimeFieldName = "CreateDT";

        public static readonly string RoadNameFieldName = "RoadName";

        public static readonly string FromNodeFieldName = "FROMNODENO";

        public static readonly string ToNodeFieldName = "TONODENO";

        public static readonly string XCoordFieldName = "XCOORD";

        public static readonly string YCoordFieldName = "YCOORD";

        public IList<IPolyline> NewRoads { get; private set; }

        public static double FragmentThreshold { get; set; }

        public Dictionary<int, IPolyline> roadsDict;

        public RoadMerger(IList<IPolyline> newRoads)
        {
            NewRoads = new List<IPolyline>(newRoads);
            for(var i=0;i<newRoads.Count;i++)
            {
                roadsDict.Add(i, newRoads[i]);
            }
        }
        #region Intersects between input polylines
        /// <summary>
        /// 将导入线打断去除自相交
        /// </summary>
        /// <param name="srcLines"></param>
        /// <returns></returns>
        public static List<IPolyline> SplitLine(List<IPolyline> srcLines)
        {
            for (var i = 0; i < srcLines.Count; i++)
            {
                var line = srcLines[i];
                var topo = line as ITopologicalOperator;
                var pts = new List<IPoint>();
                foreach (var line2 in srcLines)
                {
                    var ret = Intersect(line, line2);
                    if (ret != null) pts.AddRange(ret);
                }

                var splitted = SplitLine(srcLines[i], pts);
                if (splitted.Count > 1)
                {
                    srcLines.RemoveAt(i);
                    srcLines.AddRange(splitted);
                    i += splitted.Count - 1;
                }
            }

            return srcLines;
        }

        /// <summary>
        /// 在多个交点处打断导入线
        /// </summary>
        /// <param name="srcLine"></param>
        /// <param name="pts"></param>
        /// <returns></returns>
        private static List<IPolyline> SplitLine(IPolyline srcLine, IList<IPoint> pts)
        {
            var list = new List<IPolyline>();
            list.Add(srcLine);

            foreach (var pt in pts)
            {
                for (var i = 0; i < list.Count; i++)
                {
                    var ret = SplitLine(list[i], pt);
                    if (ret != null)
                    {
                        list.RemoveAt(i);
                        list.AddRange(ret);
                        i += ret.Count - 1;
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 在交点处打断导入线
        /// </summary>
        /// <param name="srcLine"></param>
        /// <param name="pt"></param>
        /// <returns></returns>
        private static List<IPolyline> SplitLine(IPolyline srcLine, IPoint pt)
        {
            var needSplit = false;
            var index1 = 0;
            var index2 = 0;
            srcLine.SplitAtPoint(pt, false, true, out needSplit, out index1, out index2);

            if (needSplit)
            {
                var gc = srcLine as IGeometryCollection;
                var list = new List<IPolyline>();
                for (var k = 0; k < gc.GeometryCount; k++)
                {
                    var g = gc.Geometry[k];
                    if (g is IPolyline)
                    {
                        list.Add(g as IPolyline);
                    }
                    else if (g is IPath)
                    {
                        var pl = new PolylineClass();
                        var gc2 = pl as IGeometryCollection;
                        gc2.AddGeometry(g as IPath);
                        list.Add(pl);
                    }
                    else
                    {
                        throw new NotSupportedException(string.Format("分割道路的结果类型'{0}'不被支持", g.GeometryType));
                    }
                }
                return list;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 求两条线之间的交点
        /// </summary>
        /// <param name="srcLine"></param>
        /// <param name="splitLine"></param>
        /// <returns></returns>
        private static IList<IPoint> Intersect(IPolyline srcLine, IPolyline splitLine)
        {
            var topo = srcLine as ITopologicalOperator;
            var ret = topo.Intersect(splitLine, esriGeometryDimension.esriGeometry0Dimension);
            if (ret.IsEmpty) return null;
            var pts = new List<IPoint>();
            if (ret is IPoint)
            {
                pts.Add(ret as IPoint);
            }
            else if (ret is IPointCollection)
            {
                var pc = ret as IPointCollection;
                for (var j = 0; j < pc.PointCount; j++)
                {
                    pts.Add(pc.Point[j]);
                }
            }
            else
            {
                throw new NotSupportedException(string.Format("分割道路的结果类型'{0}'不被支持", ret.GeometryType));
            }
            return pts;
        }

        #endregion

        #region 1. Find roads that intersect with others

        /// <summary>
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="roadFC"></param>
        /// <returns></returns>
        public static List<RoadCrossInfo> QueryIntersects(List<IPolyline> lines, IFeatureClass roadFC)
        {
            var list = CreateList(lines);
            list = QueryIntersects(list, roadFC);
            return list;
        }
   
        /// <summary>
        /// 创建导入线列表
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        private static List<RoadCrossInfo> CreateList(List<IPolyline> lines)
        {
            //var list = SplitLine(lines);
            var lst = new List<RoadCrossInfo>();
            for (var i = 0; i < lines.Count; i++)
            { 
                var entity = new RoadCrossInfo()
                {
                    Id = -i,
                    Text = "[新导入道路]",
                    No = "[新编号]",
                    Geometry = lines[i]
                };
            }
            return lst;
        }

        /// <summary>
        /// 将导入路线和已有路线进行叠加分析，找到交叉点
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="roadFC"></param>
        /// <returns></returns>
        private static List<RoadCrossInfo> QueryIntersects(List<RoadCrossInfo> lines, IFeatureClass roadFC)
        {
            foreach(var line in lines)
            {
                var ret = QueryIntersect(line.Geometry, roadFC);
                
                line.Crossings.AddRange(ret);
            }
            return lines;
        }

        /// <summary>
        /// 将导入路线和已有路线进行叠加分析，找到交叉点
        /// </summary>
        /// <param name="line"></param>
        /// <param name="roadFC"></param>
        /// <returns></returns>
        private static List<CrossingInfo> QueryIntersect(IPolyline line, IFeatureClass roadFC)
        {
            var cursor = roadFC.Search(new SpatialFilterClass { Geometry = line, SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects }, true);
            var topo = line as ITopologicalOperator;
            var rel = line as IRelationalOperator;
            var f = cursor.NextFeature();
            var list = new List<CrossingInfo>();
            
            while (f != null)
            {
                var shp = f.ShapeCopy;
                line.SpatialReference = shp.SpatialReference;
                var pc2 = shp as IPointCollection;
                
                var ret = topo.Intersect(shp, esriGeometryDimension.esriGeometry0Dimension);
                if (ret.IsEmpty == false)
                {
                    if (ret is IPoint || ret is IPointCollection)
                    {
                        var info = new RoadCrossInfo
                        {
                            Geometry = (IPolyline)shp,
                            Id = f.OID,
                            Text = f.get_Value(f.Fields.FindField(RoadNameFieldName)).ToString(),
                            No = f.get_Value(f.Fields.FindField(IDFieldName)).ToString()
                        };

                        if (ret is IPoint)
                        {
                            list.Add(new CrossingInfo { Crossing = (ret as IPoint), Road = info });
                        }
                        else
                        {
                            var pc = ret as IPointCollection;
                            for (var i = 0; i < pc.PointCount; i++)
                            {
                                list.Add(new CrossingInfo { Crossing = pc.Point[i], Road = info });
                            }
                        }
                    }
                    else
                    {
                        throw new NotSupportedException(string.Format("道路相交结果的类型‘{0}’不被支持", ret.GetType()));
                    }
                }
                f = cursor.NextFeature();
            }

            Marshal.ReleaseComObject(cursor);
            return list;
        }
        #endregion

        #region 2. Query and cut fragments in head and tail;
        /// <summary>
        /// 切掉路线两边出头的线头
        /// </summary>
        /// <param name="lines"></param>
        private static void CutFragments(IList<RoadCrossInfo> lines)
        {
            foreach (var line in lines) CutFragments(line);
        }
        
        /// <summary>
        /// 切掉路线两边出头的线头
        /// </summary>
        /// <param name="line"></param>
        private static void CutFragments(RoadCrossInfo line)
        {
            if (line.Enabled == false) return;

            var pts = new List<IPoint>();
            if(line.HeadCrossing != null) pts.Add(line.HeadCrossing);
            if(line.TailCrossing != null) pts.Add(line.TailCrossing);

            foreach(var pt in pts)
            {
                var ret = SplitLine(line.Geometry, pt);
                if (ret.Count > 1)
                {
                    line.Geometry = (ret[0].Length > ret[1].Length) ? ret[0] : ret[1];
                }
            }
        }

        /// <summary>
        /// 获取路线两边的线头（填充HeadCrossing和TailCrossing）
        /// </summary>
        /// <param name="lines"></param>
        public static void QueryFragments(IList<RoadCrossInfo> lines)
        {
            foreach (var line in lines) QueryFragments(line);
        }

        /// <summary>
        /// 获取路线两边的线头（填充HeadCrossing和TailCrossing）
        /// </summary>
        /// <param name="line"></param>
        private static void QueryFragments(RoadCrossInfo line)
        {
            if (line.Enabled == false) return;
            line.TailCrossing = null;
            line.HeadCrossing = null;
            double min = double.MaxValue;
            double min2 = double.MaxValue;
            var length = GetProjectedLength(line.Geometry);
            var topo = line.Geometry as ITopologicalOperator;
            var pt = new PointClass();
            for(var i=0;i<line.Crossings.Count;i++)
            {
                if(line.Crossings[i].Enabled == false) continue;

                double distance = 0, b = 0;
                bool rightSide = false;
                line.Geometry.QueryPointAndDistance(esriSegmentExtension.esriNoExtension, line.Crossings[i].Crossing, false, pt, ref distance, ref b, ref rightSide);
                distance = GetProjectedDistance(pt, distance);
                if(distance<min && distance < FragmentThreshold){
                    min = distance;
                    line.HeadCrossing = line.Crossings[i].Crossing;
                    line.HeadLength = min;
                }else if(length-distance<min2 && length-distance < FragmentThreshold){
                    min2 = length-distance;
                    line.TailCrossing = line.Crossings[i].Crossing;
                    line.TailLength = min2;
                }
            }

            if (line.TailCrossing != null)
            {
                var ret = SplitLine(line.Geometry, line.TailCrossing);
                if (ret.Count > 1)
                {
                    line.Tail = (ret[0].Length > ret[1].Length) ? ret[1] : ret[0];
                }
            }
            
            if(line.HeadCrossing != null)
            {
                var ret = SplitLine(line.Geometry, line.HeadCrossing);
                if (ret.Count > 1)
                {
                    line.Head = (ret[0].Length > ret[1].Length) ? ret[1] : ret[0];
                }
            }
        }

        private static double GetProjectedDistance(IPoint pt, double distance)
        {
            var polyline = new PolylineClass();
            var pc = polyline as IPointCollection;
            
            var pt1 = new PointClass();
            var pt2 = new PointClass();
            pt1.PutCoords(pt.X, pt.Y);
            pt2.PutCoords(pt.X + distance, pt.Y);
            pc.AddPoint(pt1);
            pc.AddPoint(pt2);
            return Math.Abs(GetProjectedLength(polyline));
        }

        private static double GetProjectedLength(IPolyline line)
        {
            var copy = new ObjectCopyClass();
            var l2 = copy.Copy(line) as IPolyline;

            var factory = new SpatialReferenceEnvironmentClass();
            var fromSR = factory.CreateGeographicCoordinateSystem((int)esriSRGeoCS3Type.esriSRGeoCS_Xian1980);//西安80
            var toSR = factory.CreateProjectedCoordinateSystem((int)esriSRProjCS4Type.esriSRProjCS_Xian1980_3_Degree_GK_Zone_40);//西安80
            l2.SpatialReference = fromSR;
            l2.Project(toSR);
            return l2.Length;
        }


        #endregion

        public static void UpdateRoads(List<RoadCrossInfo> lines, IFeatureClass fc, IFeatureClass historyFC, IFeatureClass crossFC, out List<string> newRoadIds, out Dictionary<string, string> oldRoadNewIds)
        {
            RemoveDisabledRoads(lines);
            CutFragments(lines);
            var now = DateTime.Now;
            newRoadIds = new List<string>();
            // 添加新路线
            foreach(var line in lines)
            {
                var ls = SplitLine(line.Geometry, line.Crossings.Select(x => x.Crossing).ToList());
                var ret = StoreNewRoads(ls, fc, crossFC, now);
                newRoadIds.AddRange(ret);
            }

            // 按照老路线归类交点
            var dict = new Dictionary<int, List<IPoint>>();
            var dict2 = new Dictionary<int, IPolyline>();
            foreach(var line in lines)
            {
                foreach(var cross in line.Crossings)
                {
                    if (dict.ContainsKey(cross.Road.Id) == false)
                    {
                        dict.Add(cross.Road.Id, new List<IPoint>());
                        dict2.Add(cross.Road.Id, cross.Road.Geometry);
                    }

                    dict[cross.Road.Id].Add(cross.Crossing);
                }
            }
            oldRoadNewIds = new Dictionary<string, string>();
            foreach(var pair in dict)
            {
                var id = pair.Key;
                var f = fc.GetFeature(id);
                Archive(f, historyFC);
                var ls = SplitLine(dict2[id], pair.Value);

                var ret = StoreOldRoads(ls, f, fc, crossFC, now);
                foreach(var p in ret)
                {
                    oldRoadNewIds.Add(p.Key, p.Value);
                }
            }
        }

        private static List<RoadCrossInfo> RemoveDisabledRoads(List<RoadCrossInfo> lines)
        {
            lines = lines.Where(x=>x.Enabled==true).ToList();
            foreach(var line in lines)
            {
                line.Crossings = line.Crossings.Where(x=>x.Enabled).ToList();
            }
            return lines;
        }

        private static IPolyline CopyPolyline(IPolyline line)
        {
            var objCopy = new ObjectCopyClass();
            var copy = objCopy.Copy(line) as IPolyline;
            return copy;
        }

        private static void Archive(IFeature f, IFeatureClass historyFC)
        {
            var cursor = historyFC.Insert(true);
            var buffer = historyFC.CreateFeatureBuffer();

            buffer.Shape = f.ShapeCopy;
            CopyFields(f, buffer);
            buffer.set_Value(historyFC.FindField(IDFieldName), f.get_Value(f.Fields.FindField(IDFieldName)));
            cursor.InsertFeature(buffer);
            Marshal.ReleaseComObject(cursor);
        }

        private static Dictionary<string,string> StoreOldRoads(IList<IPolyline> lines, IFeature f, IFeatureClass fc, IFeatureClass crossFC, DateTime createDate)
        {
            var ret = new Dictionary<string, string>();
            if (lines.Count < 2) return ret;
            var geo = f.ShapeCopy as IPolyline;
            
            var cursor = fc.Insert(true);
            var idIndex = cursor.FindField(IDFieldName);
            var pidIndex = cursor.FindField(ParentIDFieldName);
            var dtIndex = cursor.FindField(CreateTimeFieldName);
            var fromIndex = cursor.FindField(FromNodeFieldName);
            var toIndex = cursor.FindField(ToNodeFieldName);
            var id = GetNewId(fc);
            foreach (var line in lines)
            {
                var pc = line as IPointCollection;
                var fromNO = CreateOrGetCrossingNo(pc.Point[0], crossFC, 0.000012);
                var toNO = CreateOrGetCrossingNo(pc.Point[pc.PointCount - 1], crossFC, 0.000012);

                var buff = fc.CreateFeatureBuffer();
                ret.Add(id.ToString(), f.get_Value(idIndex).ToString());
                buff.Shape = line;
                CopyFields(f, buff);
                buff.set_Value(idIndex, id);
                buff.set_Value(pidIndex, f.get_Value(idIndex));
                buff.set_Value(dtIndex, createDate);
                
                buff.set_Value(fromIndex, fromNO);
                buff.set_Value(toIndex, toNO);
                
                cursor.InsertFeature(buff);
                cursor.Flush();
                id++;
            }

            Marshal.ReleaseComObject(cursor);
            f.Delete();
            return ret;
        }

        private static List<string> StoreNewRoads(IList<IPolyline> lines, IFeatureClass fc, IFeatureClass crossFC, DateTime createDate)
        {
            var ret = new List<string>();
            
            var cursor = fc.Insert(true);
            var idIndex = cursor.FindField(IDFieldName);
            var dtIndex = cursor.FindField(CreateTimeFieldName);
            var fromIndex = cursor.FindField(FromNodeFieldName);
            var toIndex = cursor.FindField(ToNodeFieldName);
            var id = GetNewId(fc);
            foreach (var line in lines)
            {
                var pc = line as IPointCollection;
                var fromNO = CreateOrGetCrossingNo(pc.Point[0], crossFC, 0.000012);
                var toNO = CreateOrGetCrossingNo(pc.Point[pc.PointCount - 1], crossFC, 0.000012);

                var buff = fc.CreateFeatureBuffer();
                ret.Add(id.ToString());
                buff.Shape = line;
                buff.set_Value(idIndex, id);
                buff.set_Value(dtIndex, createDate);
                buff.set_Value(fromIndex, fromNO);
                buff.set_Value(toIndex, toNO);
                cursor.InsertFeature(buff);
                cursor.Flush();
                id++;
            }

            Marshal.ReleaseComObject(cursor);
            return ret;
        }

        private static int GetNewId(IFeatureClass fc)
        {
            var sort = new TableSortClass();
            sort.Ascending[IDFieldName] = false;
            sort.Fields = IDFieldName;
            sort.Table = (ITable)fc;
            sort.Sort(null);
            var cursor = sort.Rows;
            var f = cursor.NextRow();
            if (f == null) return 1;

            var idx = cursor.FindField(IDFieldName);
            return Convert.ToInt32(f.Value[idx]) + 1;
        }

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

        /// <summary>
        /// 查询pt是否在道路交点中存在，存在则返回它的No，不存在新建后返回新的No
        /// </summary>
        /// <param name="pt"></param>
        /// <param name="fc"></param>
        /// <returns></returns>
        private static int CreateOrGetCrossingNo(IPoint pt, IFeatureClass fc, double delta)
        {
            var cursor = fc.Search(new SpatialFilterClass { Geometry = pt, SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects }, true);
            var f = cursor.NextFeature();
            if (f != null)
            {
                var no = Convert.ToInt32(f.get_Value(f.Fields.FindField(IDFieldName)));
                Marshal.ReleaseComObject(cursor);
                return no;
            }

            Marshal.ReleaseComObject(cursor);
            
            var pg = ConstructPolygon(pt, delta);

            cursor = fc.Search(new SpatialFilterClass { Geometry = pg, SpatialRel = esriSpatialRelEnum.esriSpatialRelContains }, true);
            f = cursor.NextFeature();
            if (f != null)
            {
                var no = Convert.ToInt32(f.get_Value(f.Fields.FindField(IDFieldName)));
                Marshal.ReleaseComObject(cursor);
                return no;
            }

            Marshal.ReleaseComObject(cursor);

            var id = GetNewId(fc);
            cursor = fc.Insert(true);
            var buffer = fc.CreateFeatureBuffer();
            buffer.set_Value(buffer.Fields.FindField(XCoordFieldName), pt.X);
            buffer.set_Value(buffer.Fields.FindField(YCoordFieldName), pt.Y);
            buffer.set_Value(buffer.Fields.FindField(IDFieldName), id);
            cursor.InsertFeature(buffer);
            Marshal.ReleaseComObject(cursor);
            return id;
        }

        private static IPolygon ConstructPolygon(IPoint pt, double delta)
        {
            var pg = new PolygonClass();
            var pc = pg as IPointCollection;
            var p = new PointClass { X = pt.X - delta / 2, Y = pt.Y + delta / 2 };
            pc.AddPoint(p);

            p = new PointClass { X = pt.X + delta / 2, Y = pt.Y + delta / 2 };
            pc.AddPoint(p);

            p = new PointClass { X = pt.X + delta / 2, Y = pt.Y - delta / 2 };
            pc.AddPoint(p);

            p = new PointClass { X = pt.X - delta / 2, Y = pt.Y - delta / 2 };
            pc.AddPoint(p);

            p = new PointClass { X = pt.X - delta / 2, Y = pt.Y + delta / 2 };
            pc.AddPoint(p);
            return pg;

        }

        #region History recovery
        /// <summary>
        /// 恢复到指定编号的历史记录，可以不是现状记录的直接历史
        /// </summary>
        /// <param name="parentNO"></param>
        /// <param name="fc"></param>
        /// <param name="historyFC"></param>
        /// <param name="crossFC"></param>
        /// <returns></returns>
        public static bool HistoryRecover(int parentNO, IFeatureClass fc, IFeatureClass historyFC, IFeatureClass crossFC)
        {
            var historyList = new List<int>();
            int fid;
            while(parentNO != -1)
            {
                historyList.Add(parentNO);
                parentNO = GetChildNo(parentNO, historyFC, out fid);
            }

            for(var i=0;i<historyList.Count;i++)
            {
                if (HistoryRecoverInner(historyList[historyList.Count - i - 1], fc, historyFC, crossFC) == false) return false ;
            }
            return true;
        }

        /// <summary>
        /// 获取历史记录列表
        /// </summary>
        /// <param name="fid">要获取历史记录的要素</param>
        /// <param name="fc"></param>
        /// <param name="historyFC"></param>
        /// <returns>历史记录列表，每一个值都是objectid，而不是道路编号</returns>
        public static List<int> GetHistoryList(int fid, IFeatureClass fc, IFeatureClass historyFC)
        {
            var f = fc.GetFeature(fid);
            if (f == null) return null;

            var historyList = new List<int>();
            var o = f.get_Value(f.Fields.FindField(ParentIDFieldName));
            if (o == null || o == DBNull.Value) return historyList;

            var parentNo = Convert.ToInt32(o);
            int id;
            while(parentNo != -1)
            {
                parentNo = GetParentNo(parentNo, historyFC, out id);
                if (parentNo != -1) historyList.Add(id);    
            }
            return historyList;
        }

        private static int GetParentNo(int no, IFeatureClass fc, out int fid)
        {
            var cursor = fc.Search(new QueryFilterClass { WhereClause = string.Format("{0} = {1}", IDFieldName, no) }, true);
            var f = cursor.NextFeature();

            var id = f == null ? -1 : Convert.ToInt32(f.get_Value(f.Fields.FindField(ParentIDFieldName)));
            fid = f == null ? -1 : f.OID;
            Marshal.ReleaseComObject(cursor);
            return id;
        }

        private static int GetChildNo(int parentNO, IFeatureClass fc, out int fid)
        {
            var cursor = fc.Search(new QueryFilterClass { WhereClause = string.Format("{0} = {1}", ParentIDFieldName, parentNO) }, true);
            var f = cursor.NextFeature();
            
            var id = f == null ? -1 : Convert.ToInt32(f.get_Value(f.Fields.FindField(IDFieldName)));
            fid = f == null ? -1 : f.OID;
            Marshal.ReleaseComObject(cursor);
            return id;
        }

        /// <summary>
        /// 将现状恢复到上一次历史状态
        /// </summary>
        /// <param name="parentNo"></param>
        /// <param name="fc"></param>
        /// <param name="historyFC"></param>
        /// <returns>上一次历史状态的id</returns>
        private static bool HistoryRecoverInner(int parentNO, IFeatureClass fc, IFeatureClass historyFC, IFeatureClass crossFC)
        {
            // 历史表里没这个parentNo
            var cursor = historyFC.Search(new QueryFilterClass { WhereClause = string.Format("{0} = {1}", IDFieldName, parentNO) }, true);
            var f = cursor.NextFeature();
            if (f == null)
            {
                Marshal.ReleaseComObject(cursor);
                return false;
            }

            // 当前表的记录的直接历史并不是parentNo对应的历史
            var cursor3 = fc.Update(new QueryFilter { WhereClause = string.Format("{0} = {1}", ParentIDFieldName, parentNO) }, true);
            f = cursor3.NextFeature();
            if( f == null)
            {
                Marshal.ReleaseComObject(cursor3);
                return false;
            }

            // 记录两端节点，不删除它们
            var nodes = new List<int>();
            var o = f.get_Value(f.Fields.FindField(FromNodeFieldName));
            if(o != null && o != DBNull.Value) nodes.Add(Convert.ToInt32(o));
            o = f.get_Value(f.Fields.FindField(ToNodeFieldName));
            if(o != null && o != DBNull.Value) nodes.Add(Convert.ToInt32(o));

            // 将记录从历史表拷贝到当前表，并删除历史表中的记录
            var cursor2 = fc.Insert(true);
            var buffer = fc.CreateFeatureBuffer();
            CopyFields(f, buffer);
            buffer.set_Value(buffer.Fields.FindField(IDFieldName), parentNO);
            cursor2.InsertFeature(buffer);

            Marshal.ReleaseComObject(cursor2);
            f.Delete();
            Marshal.ReleaseComObject(cursor);
            
            // 将当前表中的相关记录删除
            var fromIndex = cursor3.FindField(FromNodeFieldName);
            var toIndex = cursor3.FindField(ToNodeFieldName);
            var nodesToRemove = new HashSet<int>();
            while(f != null)
            {
                o = f.get_Value(fromIndex);
                if(o!= null && o != DBNull.Value) 
                {
                    if(nodes.Contains(Convert.ToInt32(o)) == false) // 是中间节点，需要删除；否则是两端节点，需要保留
                    {
                        nodesToRemove.Add(Convert.ToInt32(o));
                    }
                }
                o = f.get_Value(toIndex);
                if(o!= null && o != DBNull.Value) 
                {
                    if(nodes.Contains(Convert.ToInt32(o)) == false) // 是中间节点，需要删除；否则是两端节点，需要保留
                    {
                        nodesToRemove.Add(Convert.ToInt32(o));
                    }
                }

                cursor3.DeleteFeature();
                f = cursor3.NextFeature();
            }
            Marshal.ReleaseComObject(cursor3);

            // 删除相关节点
            foreach(var i in nodesToRemove)
            {
                DeleteNode(i, parentNO, crossFC, fc);
            }
            return true;
        }

        private static void DeleteNode(int nodeNO, int parentRoadNo, IFeatureClass nodeFC, IFeatureClass roadFC)
        {
            // 这个节点还关联了别的道路，并不仅仅在这条历史记录相关的道路上
            var cursor = roadFC.Search(new QueryFilterClass{ WhereClause=string.Format("({0} = {2} OR {1} = {2}) AND {3} <> {4}", FromNodeFieldName, ToNodeFieldName, nodeNO, ParentIDFieldName, parentRoadNo)}, true);
            var f = cursor.NextFeature();
            if(f == null)
            {
                var pQueryFilter = new QueryFilterClass();
                pQueryFilter.WhereClause = string.Format("{0} = {1}", IDFieldName, nodeNO);
                var pTable = nodeFC as ITable;
                pTable.DeleteSearchedRows(pQueryFilter);
                
            }
            Marshal.ReleaseComObject(cursor);
        }

        #endregion
    }
}
