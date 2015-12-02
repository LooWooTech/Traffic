using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LoowooTech.Traffic.Models
{
    public enum InquiryMode
    {
        None,
        Filter,
        Search,
        Statistic,
        Region,
        Way
    }
    public enum OperateMode
    {
        None,
        Add,
        Delete,
        Edit
    }
}
