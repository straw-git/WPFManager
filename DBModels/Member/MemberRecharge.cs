
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBModels.Member
{
    /// <summary>
    /// 会员充值
    /// </summary>
    public class MemberRecharge
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int MemberId { get; set; }//会员
        public int CustomerId { get; set; }//顾客

        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(200)]
        public string QuickCode { get; set; }
        [MaxLength(50)]
        public string IdCard { get; set; }//身份证号

        public decimal OldPrice { get; set; }//原余额
        public decimal NewPrice { get; set; }//新余额
        public decimal Price { get; set; }//充值金额

        public int PayOrderId { get; set; }//付款订单

        public int Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
