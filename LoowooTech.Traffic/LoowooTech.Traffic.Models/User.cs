using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace LoowooTech.Traffic.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        [Column(TypeName="int")]
        public Role Role { get; set; }
    }

    public enum Role
    {
        [Description("普通用户")]
        Common,
        [Description("管理员")]
        Admin
    }
}
