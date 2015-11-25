using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace LoowooTech.Traffic.Models
{
    public enum DataType
    {
        [Description("路网")]
        Road,
        [Description("公交路线")]
        BusLine,
        [Description("公交站点")]
        BusStop,
        [Description("停车场")]
        Parking
    }
    public enum ConditionNumber
    {
        One,
        Two,
        Three
    }
}
