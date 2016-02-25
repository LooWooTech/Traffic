using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Traffic.Common
{
    public class RoadCrossInfo
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public string No { get; set; }

        public List<CrossingInfo> Crossings { get; set; }
        
        public bool Enabled { get; set; }

        public IPolyline Geometry { get; set; }

        public RoadCrossInfo()
        {
            Enabled = true;
            Crossings = new List<CrossingInfo>();
        }

        /// <summary>
        /// 是由哪个交点切出的尾部碎片
        /// </summary>
        public IPoint TailCrossing { get; set; }
        public double TailLength { get; set; }
        public IPolyline Tail { get; set; }
        /// <summary>
        /// 是由哪个交点切出的头部碎片
        /// </summary>
        public IPoint HeadCrossing { get; set; }
        public double HeadLength { get; set; }
        public IPolyline Head { get; set; }
    }

    public class CrossingInfo
    {
        public IPoint Crossing { get; set; }

        public bool Enabled { get; set; }

        public RoadCrossInfo Road { get; set; }

        public CrossingInfo()
        {
            Enabled = true;
        }
    }
}
