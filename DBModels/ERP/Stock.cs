
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBModels.ERP
{
    /// <summary>
    /// 库存
    /// </summary>
    public class Stock
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(50)]
        public string GoodsId { get; set; }
        public int StoreId { get; set; }//仓库
        public int Count { get; set; }
    }
}
