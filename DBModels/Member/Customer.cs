
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBModels.Member
{
    /// <summary>
    /// 顾客
    /// </summary>
    public class Customer
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(200)]
        public string QuickCode { get; set; }
        [MaxLength(50)]
        public string IdCard { get; set; }//身份证号
        [MaxLength(10)]
        public string Sex { get; set; }//性别
        [MaxLength(20)]
        public string Birthday { get; set; }//生日
        [MaxLength(200)]
        public string Address { get; set; }//户籍地址
        [MaxLength(200)]
        public string AddressNow { get; set; }//现住址
        [MaxLength(20)]
        public string Phone { get; set; }//电话号码

        [MaxLength(10)]
        public string PromotionCode { get; set; }//推广码

        [MaxLength(10)]
        public string BePromotionCode { get; set; }//被推广码

        public bool IsMember { get; set; }//是否是会员

        public DateTime CreateTime { get; set; }//创建时间
        public int Creater { get; set; }//创建人
    }
}
