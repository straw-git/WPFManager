
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBModels.ERP
{
    /// <summary>
    /// 采购计划
    /// </summary>
    public class PurchasePlan
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(50)]
        public string PlanCode { get; set; }//单号

        public bool Finished { get; set; }//采购状态 是否完成
        public bool Stock { get; set; }//是否已入库

        public bool IsDel { get; set; }
        public int DelUser { get; set; }
        public DateTime DelTime { get; set; }

        public int Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
