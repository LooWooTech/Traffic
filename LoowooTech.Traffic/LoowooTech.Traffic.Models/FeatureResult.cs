using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Traffic.Models
{
    public class FeatureResult
    {
        public string RoadWhereClause { get; set; }
        public IFeature Feature { get; set; }
        public string StopWhereClause { get; set; }
        public List<IFeature> Stops { get; set; }
        public string StartName { get; set; }
        public string EndName { get; set; }
        public string StartEndWhereClause { get; set; }
    }
}
