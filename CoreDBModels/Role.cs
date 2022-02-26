using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreDBModels
{
    /// <summary>
    /// 角色表
    /// </summary>
    public class Role
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }

        public bool IsDel { get; set; }
        public int DelUser { get; set; }
        [MaxLength(50)]
        public string DelUserName { get; set; }
        public DateTime DelTime { get; set; }
    }
}
