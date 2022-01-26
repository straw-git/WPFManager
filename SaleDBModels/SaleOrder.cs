
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaleDBModels
{
    /// <summary>
    /// 订单表
    /// </summary>
    public class SaleOrder
    {
        [Key]
        [MaxLength(50)]
        public string Id { get; set; }//编号 个人Id+时间

        public int CustomerId { get; set; }//关联的客户 
        public int CustomerTempId { get; set; }//关联的临时客户  

        [MaxLength(50)]
        public string Name { get; set; }//客户名  
        [MaxLength(200)]
        public string QuickCode { get; set; }//简码  
        [MaxLength(50)]
        public string IdCard { get; set; }//身份证号 
        [MaxLength(50)]
        public string Phone { get; set; }//电话号
        public bool IsMember { get; set; }//此时 是否是会员

        //订单相关
        public string OrderId { get; set; }//订单编号
        public bool OrderFinished { get; set; }//订单是否已完成
        public DateTime OrderFinishedTime { get; set; }//订单完成的时间
        public int OrderFinishUserId { get; set; }//订单操作人编号
        [MaxLength(50)]
        public string OrderFinishUserName { get; set; }//订单操作人名称
        //收费相关
        public bool IsPay { get; set; }//是否已支付
        public decimal PayPrice { get; set; }//支付金额

        public DateTime CreateTime { get; set; }//创建时间
        public int Creater { get; set; }//创建人
        public string CreatorName { get; set; }//创建人名称
    }
}
