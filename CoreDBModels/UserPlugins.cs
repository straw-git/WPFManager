using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreDBModels
{
    /// <summary>
    /// 作为用户权限外的权限
    /// </summary>
    public class UserPlugins
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int UserId { get; set; }//用户Id
        public string IncreasePages { get; set; }//在基础权限上增加的页面Id 以，分隔
        public string DecrementPages { get; set; }//在基础权限上减少的页面Id 以，分隔

        public DateTime UpdateTime { get; set; }//最后更新时间
    }
}
