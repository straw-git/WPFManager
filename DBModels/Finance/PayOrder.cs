
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBModels.Finance
{
    /// <summary>
    /// 付款账单
    /// </summary>
    public class PayOrder
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(50)]
        public string Code { get; set; }//账单号
        public int Type { get; set; }//付款方式
        public decimal Price { get; set; }//付款金额

        public bool IsDel { get; set; }
        public int DelUser { get; set; }
        public DateTime DelTime { get; set; }

        public int Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
