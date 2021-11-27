using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBModels.Member
{
    public class Member
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
        [MaxLength(10)]
        public string Sex { get; set; }//性别
        [MaxLength(20)]
        public string Birthday { get; set; }//生日
        [MaxLength(20)]
        public string Phone { get; set; }//电话号码

        [MaxLength(50)]
        public string CardNumber { get; set; }//卡号
        public decimal Price { get; set; }//账户余额
        public decimal RechargeCount { get; set; }//充值总金额

        public bool IsDelete { get; set; }

        public int Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
