
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinanceDBModels.Models
{
    /// <summary>
    /// 支付
    /// </summary>
    public class Payment
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string CardId { get; set; }
        public string Remark { get; set; }
        public string Holder { get; set; }//持有人姓名
        public int PayModelId { get; set; }//支付方式Id
        public decimal Price { get; set; }//初始余额

        public bool IsDel { get; set; }
        public int DelUser { get; set; }
        public DateTime DelTime { get; set; }

        public int Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
