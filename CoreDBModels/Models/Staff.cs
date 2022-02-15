using System;
using System.ComponentModel.DataAnnotations;

namespace CoreDBModels.Models
{
    /// <summary>
    /// 员工
    /// </summary>
    public class Staff
    {
        [Key]
        [MaxLength(50)]
        public string Id { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(200)]
        public string QuickCode { get; set; }
        [MaxLength(50)]
        public string IdCard { get; set; }
        public int Sex { get; set; }
        [MaxLength(500)]
        public string Address { get; set; }
        [MaxLength(500)]
        public string NowAddress { get; set; }
        [MaxLength(50)]
        public string Phone { get; set; }
        [MaxLength(100)]
        public string QQ { get; set; }

        public int JobPostId { get; set; }//职位

        public bool IsDel { get; set; }
        public int DelUser { get; set; }
        public DateTime DelTime { get; set; }

        public DateTime Register { get; set; }//入职时间
        public int Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
