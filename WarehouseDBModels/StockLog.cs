using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehouseDBModels
{
    /// <summary>
    /// 库存日志
    /// </summary>
    public class StockLog
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int StoreId { get; set; }//仓库
        public int StockType { get; set; }//库存类型  0入库 1出库

        [MaxLength(50)]
        public string GoodsId { get; set; }//物品Id

        public int Count { get; set; }
        public int Surplus { get; set; }//操作后剩余
        public decimal SalePrice { get; set; } //当时的销售价格 自动读取Goods表中数据
        public decimal Price { get; set; }//进货价

        public int SupplierId { get; set; }//供应商
        [MaxLength(200)]
        public string Manufacturer { get; set; }//生产厂家
        [MaxLength(200)]
        public string Remark { get; set; }//备注

        public int Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
