using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRDBModels
{
    /// <summary>
    /// 工资
    /// </summary>
    public class StaffSalary
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(50)]
        public string StaffId { get; set; }//员工
        [MaxLength(50)]
        public string SatffName { get; set; }
        [MaxLength(200)]
        public string StaffQuickCode { get; set; }
        public DateTime Register { get; set; }//注册时间

        public DateTime Start { get; set; }//开始时间
        public bool IsEnd { get; set; }//是否有结束时间
        public DateTime End { get; set; }//结束时间
        public decimal Price { get; set; }
        [MaxLength(200)]
        public string Remark { get; set; }

        public int Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
