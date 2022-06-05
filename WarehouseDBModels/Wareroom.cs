using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarehouseDBModels
{
    /// <summary>
    /// 库房
    /// </summary>
    public class Wareroom
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int WareroomTypeId { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }//库房名称
        public int ParentId { get; set; }//父级 ==0时 为仓库
    }
}
