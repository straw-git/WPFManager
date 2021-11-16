
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBModels.ERP
{
    /// <summary>
    /// 采购计划详情
    /// </summary>
    public class PurchasePlanItem
    {
        [Key]
        [MaxLength(50)]
        public string Id { get; set; }
        [MaxLength(50)]
        public string PlanCode { get; set; }//单号
        [MaxLength(50)]
        public string GoodsId { get; set; }//物品
        public int Count { get; set; }//总量
        public int Finished { get; set; }//完成量
    }
}
