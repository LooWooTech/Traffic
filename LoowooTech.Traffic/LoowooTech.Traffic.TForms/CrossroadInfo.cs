using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Traffic.TForms
{
    /// <summary>
    /// 路线交叉点
    /// </summary>
    public class CrossroadInfo
    {
        /// <summary>
        /// 坐标
        /// </summary>
        public IPoint Point { get; set; }

        /// <summary>
        /// OID
        /// </summary>
        public int OID { get; set; }

        /// <summary>
        /// 相交道路名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 相交道路编号
        /// </summary>
        public int NO { get; set; }
    }
}
