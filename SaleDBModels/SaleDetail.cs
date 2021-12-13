
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaleDBModels
{
    /// <summary>
    /// 订单详情
    /// </summary>
    public class SaleDetail
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int OrderType { get; set; }//0:订单入口 1:单品入口
        public string OrderId { get; set; }//订单编号
        [MaxLength(50)]
        public string GoodsId { get; set; }//物品Id
        [MaxLength(100)]
        public string GoodsName { get; set; }//物品名称

        public decimal Price { get; set; }//物品单价
        public decimal TotalPrice { get; set; }//总价

        public int Count { get; set; }//数量

        public bool Finished { get; set; }//是否已完成
        [MaxLength(50)]
        public string CreatorName { get; set; }
        public int CreatorId { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
