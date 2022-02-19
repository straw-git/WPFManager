
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerDBModels
{
    /// <summary>
    /// 满减活动
    /// </summary>
    public class MJActivity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public decimal M { get; set; }//满多少
        public decimal J { get; set; }//减多少
        public int Type { get; set; }//满减方式 0：消费活动  1：充值活动
        public int MemberTypeId { get; set; }//会员类型 

        public int Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
