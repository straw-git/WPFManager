using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ERPDBModels
{
    /// <summary>
    /// 供应商
    /// </summary>
    public class Supplier
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Type { get; set; }//类型 1：单位 2：个人
        [MaxLength(200)]
        public string Address { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }//公司
        public bool Qualification { get; set; }//供货资质

        public string Phone { get; set; }//联系电话
        public string ContactName { get; set; }//联系人

        public int Creater { get; set; }
        public DateTime CreateTime { get; set; }
        public bool IsDel { get; set; }
    }
}
