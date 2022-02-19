
using System;
using System.ComponentModel.DataAnnotations;


namespace FixedAssetsDBModels
{
    /// <summary>
    /// 固定资产
    /// </summary>
    public class FixedAssets
    {
        [Key]
        [MaxLength(50)]
        public string Id { get; set; }//资产编号
        public int State { get; set; }//状态 dic
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(50)]
        public string ModelName { get; set; }//型号
        [MaxLength(50)]
        public string UnitName { get; set; }//单位

        public int Location { get; set; }// 位置 dic
        [MaxLength(50)]
        public string PrincipalName { get; set; }//负责人名称
        [MaxLength(50)]
        public string PrincipalPhone { get; set; }//负责人电话

        public DateTime Register { get; set; }//资产注册时间
        public DateTime LastCheck { get; set; }//最后一次盘点时间

        public int Count { get; set; }//数量
        [MaxLength(200)]
        public string From { get; set; }//来源


        public bool IsDel { get; set; }
        public int DelUser { get; set; }
        public DateTime DelTime { get; set; }

        public int Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
