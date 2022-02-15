using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SaleDBModels.Models
{
    /// <summary>
    /// 挂存临时表（挂存表可查看价格趋势）
    /// </summary>
    public class TempSingleOrder
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(100)]
        public string Remark { get; set; }//挂存的备注
        public decimal TotalPrice { get; set; }//挂存时的总金额 用于对比价格优势

        [MaxLength(50)]
        public string CreatorName { get; set; }
        public int CreatorId { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
