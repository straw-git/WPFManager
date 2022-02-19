using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreDBModels
{
    /// <summary>
    /// 暂时没有使用
    /// </summary>
    public class Menu
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [MaxLength(50)]
        public string Code { get; set; }
        [MaxLength(50)]
        public string ParentCode { get; set; }
        [MaxLength(50)]
        public string Title { get; set; }
        [MaxLength(200)]
        public string Url { get; set; }

        public int Type { get; set; }// 类型 0：导航 1：按钮 2：控件
        public string TypeName { get; set; } //当前项对应的控件名称 用于授权时使用

        public int Order { get; set; }
    }
}
