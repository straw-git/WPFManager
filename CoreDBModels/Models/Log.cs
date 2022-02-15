
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreDBModels.Models
{
    public class Log
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(50)]
        public string Type { get; set; }
        [MaxLength(200)]
        public string LogInfo { get; set; }

        public int Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
