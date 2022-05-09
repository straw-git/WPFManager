using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreDBModels
{
    /// <summary>
    /// 日志
    /// </summary>
    public class Log
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int LogType { get; set; }//日志类型
        public string LogStr { get; set; }//日志内容

        public int Creator { get; set; }//创建人
        public DateTime CreateTime { get; set; }//创建时间
    }
}
