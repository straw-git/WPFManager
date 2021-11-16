
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DBModels.Finance
{
    public class FinanceBill
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int AddType { get; set; }//记账类型 
        [MaxLength(500)]
        public string Things { get; set; }
        [MaxLength(200)]
        public string Remark { get; set; }

        public DateTime BillTime { get; set; }
        [MaxLength(50)]
        public string StaffId { get; set; }
        [MaxLength(50)]
        public string StaffName { get; set; }
        [MaxLength(200)]
        public string StaffQuickCode { get; set; }

        public decimal Price { get; set; }

        public int Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
