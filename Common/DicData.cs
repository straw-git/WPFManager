using CoreDBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class DicData
{
    /// <summary>
    /// 角色
    /// </summary>
    public static readonly string Role = "角色".Convert2Pinyin();
    /// <summary>
    /// 组织架构
    /// </summary>
    public static readonly string JobPost = "组织架构".Convert2Pinyin();
    /// <summary>
    /// 支付方式
    /// </summary>
    public static readonly string PayModel = "支付方式".Convert2Pinyin();
    /// <summary>
    /// 供应商类型
    /// </summary>
    public static readonly string SupplierType = "供应商类型".Convert2Pinyin();
    /// <summary>
    /// 物品类型
    /// </summary>
    public static readonly string GoodsType = "物品类型".Convert2Pinyin();
    /// <summary>
    /// 物品单位
    /// </summary>
    public static readonly string GoodsUnit = "物品单位".Convert2Pinyin();
    /// <summary>
    /// 仓库
    /// </summary>
    public static readonly string Store = "仓库".Convert2Pinyin();
    /// <summary>
    /// 固定资产状态
    /// </summary>
    public static readonly string FixedAssetsState = "固定资产状态".Convert2Pinyin();
    /// <summary>
    /// 固定资产位置
    /// </summary>
    public static readonly string FixedAssetsLocation = "固定资产位置".Convert2Pinyin();
}
