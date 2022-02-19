
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FixedAssetsDBModels
{
    /// <summary>
    /// 固定资产盘点
    /// </summary>
    public class FixedAssetsCheck
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(50)]
        public string FixedAssetsCode { get; set; }//固定资产编号

        public int OldCount { get; set; }
        public int NewCount { get; set; }

        public int OldState { get; set; }
        public int NewState { get; set; }

        public int OldLocation { get; set; }
        public int NewLocation { get; set; }

        [MaxLength(50)]
        public string OldPrincipalName { get; set; }//负责人名称
        [MaxLength(50)]
        public string NewPrincipalName { get; set; }

        [MaxLength(50)]
        public string OldPrincipalPhone { get; set; }//负责人电话
        [MaxLength(50)]
        public string NewPrincipalPhone { get; set; }

        [MaxLength(50)]
        public string StaffId { get; set; }//盘点人Id
        [MaxLength(50)]
        public string StaffName { get; set; }//盘点人
        public DateTime Check { get; set; }//盘点时间
        [MaxLength(200)]
        public string Remark { get; set; }

        public int Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
