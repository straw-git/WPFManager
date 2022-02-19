
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerDBModels
{
    /// <summary>
    /// 顾客临时（来访记录）
    /// </summary>
    public class CustomerTemp
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int CustomerId { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(200)]
        public string QuickCode { get; set; }
        [MaxLength(50)]
        public string IdCard { get; set; }//身份证号
        [MaxLength(50)]
        public string Phone { get; set; }//电话号

        public bool IsMember { get; set; }//此时 是否是会员

        public decimal TW { get; set; } //体温
        public bool XCM { get; set; }//行程码

        public DateTime CreateTime { get; set; }//创建时间
        public int Creater { get; set; }//创建人
    }
}
