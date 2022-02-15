using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaleDBModels.Models
{
    /// <summary>
    /// 常用项详情
    /// </summary>
    public class UserCommonItem
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(50)]
        public string GoodsId { get; set; }//物品
        [MaxLength(100)]
        public string GoodsName { get; set; }

        public int Count { get; set; }
    }
}
