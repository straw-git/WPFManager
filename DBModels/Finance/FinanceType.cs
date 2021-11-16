using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DBModels.Finance
{
    /// <summary>
    /// 财务类型
    /// </summary>
    public class FinanceType
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }//类型名称
        public int TypeId { get; set; }//财务类型类型  0:进项 1：出项

        public int Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
