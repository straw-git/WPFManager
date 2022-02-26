using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreDBModels
{
    public class UserPlugins
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int UserId { get; set; }//用户Id
        public string Pages { get; set; }//页面Id 以，分隔

        public DateTime UpdateTime { get; set; }//最后更新时间
    }
}
