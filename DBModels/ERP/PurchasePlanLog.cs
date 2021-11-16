
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBModels.ERP
{
    public class PurchasePlanLog
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(50)]
        public string ItemId { get; set; }

        public int Count { get; set; }
        public decimal PurchasePrice { get; set; }//采购单价
        public int SupplierId { get; set; }//供应商
        [MaxLength(200)]
        public string Manufacturer { get; set; }//生产厂家
        [MaxLength(200)]
        public string Remark { get; set; }//备注

        public int Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
