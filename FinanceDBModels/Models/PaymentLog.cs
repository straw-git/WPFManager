
using System;

namespace FinanceDBModels.Models
{
    /// <summary>
    /// 支付详情
    /// </summary>
    public class PaymentLog
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int PayModelId { get; set; }//支付方式Id
        public int PaymentId { get; set; }//支付账号Id
        public decimal Price { get; set; }//账户余额 

        public string UseWindowName { get; set; }//使用页面名称

        public int Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
