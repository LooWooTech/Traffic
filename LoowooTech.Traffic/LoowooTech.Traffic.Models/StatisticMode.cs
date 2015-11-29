using System.ComponentModel;

namespace LoowooTech.Traffic.Models
{
    public enum StatisticMode
    {
        [Description("六片区")]
        Region,
        [Description("错峰限行")]
        Restrict,
        [Description("老城区")]
        Old,
        [Description("全市区")]
        All
    }
}
