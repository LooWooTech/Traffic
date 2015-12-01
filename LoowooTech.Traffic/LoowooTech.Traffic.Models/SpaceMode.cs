using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace LoowooTech.Traffic.Models
{
    public enum SpaceMode
    {
        [Description("相交")]
        Intersect,
        [Description("相接")]
        Touches,
        [Description("覆盖")]
        Overlaps,
        [Description("跨越")]
        Crossed,
        [Description("被包含")]
        Within,
        [Description("包含")]
        Contains
    }
}
