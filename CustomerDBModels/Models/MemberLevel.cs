using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerDBModels.Models
{
    /// <summary>
    /// 会员等级
    /// </summary>
    public class MemberLevel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }//等级名称

        public decimal LogPriceCount { get; set; }//历史充值总金额
    }
}
