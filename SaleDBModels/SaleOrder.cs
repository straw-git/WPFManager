
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaleDBModels
{
    /// <summary>
    /// 订单表
    /// </summary>
    public class SaleOrder
    {
        [Key]
        [MaxLength(50)]
        public string Id { get; set; }//编号 个人Id+时间
        public decimal Price { get; set; }//金额
        public bool Finished { get; set; }//是否已完成
        [MaxLength(50)]
        public string CreatorName { get; set; }
        public int CreatorId { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
