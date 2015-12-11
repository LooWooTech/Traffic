using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace LoowooTech.Traffic.Models
{
    public enum DataType
    {
        [Description("路网相关")]
        Road,
        [Description("公交相关")]
        BusLine,
        [Description("公交站点")]
        BusStop,
        [Description("停车设施相关")]
        Parking,
        [Description("公共自行车相关")]
        Bike,
        [Description("交通流量检测器")]
        Flow
    }
    public enum ConditionNumber
    {
        One,
        Two,
        Three
    }
}
