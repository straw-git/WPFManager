
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaleDBModels.Models
{
    /// <summary>
    /// 常用项
    /// </summary>
    public class UserCommonObj
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }//常用项名称
        public int Type { get; set; }//常用项类型 0:公共 1：私人
        [MaxLength(200)]
        public string Remark { get; set; }//常用项备注


        public int Creator { get; set; }//创建人（私人类型时的拥有者）
        public DateTime CreateTime { get; set; }
    }
}
