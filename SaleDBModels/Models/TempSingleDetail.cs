using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaleDBModels.Models
{
    public class TempSingleDetail
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int OrderId { get; set; }//挂存表的Id
        [MaxLength(50)]
        public string GoodsId { get; set; }//物品
        public int Count { get; set; }//挂存时的数量
        public decimal Price { get; set; }//挂存时的价格
    }
}
