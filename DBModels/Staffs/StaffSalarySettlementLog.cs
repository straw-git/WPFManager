
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBModels.Staffs
{
    /// <summary>
    /// 工资月结详情
    /// </summary>
    public class StaffSalarySettlementLog
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(50)]
        public string StaffId { get; set; }//员工
        [MaxLength(50)]
        public string StaffName { get; set; }
        [MaxLength(200)]
        public string StaffQuickCode { get; set; }

        [MaxLength(50)]
        public string MonthCode { get; set; }//StaffWageSettlement 表主键

        public decimal BasePrice { get; set; }//基础工资
        public decimal SalePrice { get; set; }//提成
        public decimal InsurancePrice { get; set; }//保险 社保+商业
        public decimal DeductionPrice { get; set; }//扣费合计
        public decimal AwardPrice { get; set; }//奖励合计
        public decimal Price { get; set; }//工资总额

        public int Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
