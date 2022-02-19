using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRDBModels
{
    /// <summary>
    /// 员工劳动合同
    /// </summary>
    public class StaffContract
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime Start { get; set; }//合同开始时间
        public DateTime End { get; set; }//合同截止时间
        public DateTime Write { get; set; }//签订时间
        public decimal Price { get; set; } //合同金额
        
        [MaxLength(200)]
        public string Remark { get; set; }

        [MaxLength(50)]
        public string StaffId { get; set; }

        public bool Stop { get; set; }
        public DateTime StopTime { get; set; }
        public int StopUser { get; set; }

        public int Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
