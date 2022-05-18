using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CoreDBModels
{
    /// <summary>
    /// 职位更改日志
    /// </summary>
    public class PositionChangeLog
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int OldId { get; set; }//原职位Id
        public int NewId { get; set; }//新职位Id
        [MaxLength(200)]
        public string Remark { get; set; }//变动备注
        public DateTime StartTime { get; set; }//生效时间
    }
}
