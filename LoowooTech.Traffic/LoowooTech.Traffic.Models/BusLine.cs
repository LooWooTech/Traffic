using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace LoowooTech.Traffic.Models
{
    [Table("BusLine")]
    public class BusLine
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string LineName { get; set; }
        public string ShortName { get; set; }
        [Column(TypeName="int")]
        public DirectType Direction { get; set; }
        public string DirectionName { get; set; }
        public string StartStop { get; set; }
        public string EndStop { get; set; }
        [NotMapped]
        public IFeature Feature { get; set; }
        
    }

    public enum DirectType
    {
        [Description("环线")]
        Loop,
        [Description("上行")]
        Up,
        [Description("下行")]
        Down
    }
}
