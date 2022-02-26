using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreDBModels
{
    /// <summary>
    /// 角色插件权限
    /// </summary>
    public class RolePlugins
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int RoleId { get; set; }//角色Id
        [MaxLength(200)]
        public string Pages { get; set; }//页面Id 以，分隔

        public DateTime UpdateTime { get; set; }//最后更新时间
    }
}
