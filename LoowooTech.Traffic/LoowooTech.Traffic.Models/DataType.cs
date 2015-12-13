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
        Flow,
        [Description("人口岗位相关")]
        People
    }
    public enum PeoplePost
    {
        [Description("现状人口")]
        CurrentPeople,
        [Description("现状人口密度")]
        CurrentPropleDensity,
        [Description("规划人口")]
        PlanPeople,
        [Description("规划人口密度")]
        PlanPeopleDensity,
        [Description("现状岗位")]
        CurrentPost,
        [Description("现状岗位密度")]
        CurrentPostDensity,
        [Description("规划岗位")]
        PlanPost,
        [Description("规划岗位密度")]
        PlanPostDensity
    }
    public enum ConditionNumber
    {
        One,
        Two,
        Three
    }
}
