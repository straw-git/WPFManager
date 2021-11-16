
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBModels.Member
{
    /// <summary>
    /// 顾客临时（来访记录）
    /// </summary>
    public class CustomerTemp
    {
        [Key]
        public string Id { get; set; }

        public int CustomerId { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(200)]
        public string QuickCode { get; set; }
        [MaxLength(50)]
        public string IdCard { get; set; }//身份证号

        public bool IsMember { get; set; }//此时 是否是会员

        public decimal TW { get; set; } //体温
        public bool HS { get; set; }//核酸

        public DateTime CreateTime { get; set; }//创建时间
        public int Creater { get; set; }//创建人
    }
}
