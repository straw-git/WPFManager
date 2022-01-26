# WPF开发的管理系统（数据库MSSqlserver）

项目采用插件式方式开发,在开发时可以更好的业务分离,提高多人协作开发效率.

# 项目中使用到的技术

NETFramework472  
Panuon.UI.Silver  
LiveCharts  
EntityFramework CodeFirst  
NPOI.Excel  

开发环境：  
VS2019+Sqlserver2019

# 正在进行中

CustomerTemps数据表更改 添加是否完成  
创建订单  
单品收银、订单收银  
附件管理（计划中 使用 .Net Core3.1 WebAPI）  

# 示例文章

[客户管理插件文章](https://zhuanlan.zhihu.com/p/439497177)    
[搭建及扩展文章](https://zhuanlan.zhihu.com/p/428356007)  
[基础功能示例文章](https://zhuanlan.zhihu.com/p/431962796)

# 基础目录介绍

DBs/DBModels ： 基础数据实体类库[DbContext文件为 DBModels.DBContext]  
				 包含：  
					 Activities 活动   
						 MJActivity 满减活动  
					 ERP 进销存  
						 Goods 物品  
						 PurchasePlan 采购计划  
						 PurchasePlanItem 采购计划详情  
						 PurchasePlanLog 实际采购详情  
						 Stock 库存  
						 StockLog 库存日志  
						 Supplier 供应商  
					 Finance 财务  
						 FinanceBill 财务账单  
						 FinanceType 财务类型  
						 Payment 支付  
						 PaymentLog 支付日志  
						 PayOrder 付款账单  
					 Member 用户  
						 Customer 顾客  
						 CustomerTemp 顾客临时表（来访记录）  
						 Member 会员  
						 MemberLevel 会员等级  
						 MemberRecharge 会员充值记录  
					 Staffs 员工  
						 Staff 员工信息  
						 StaffContract 合同  
						 StaffInsurance 保险  
						 StaffSalary 工资  
						 StaffSalaryOther 奖罚  
						 StaffSalarySettlement 工资结算  
						 StaffSalarySettlementLog 工资结算日志  
					 Sys 系统  
						 Attachment 附件表
						 Log 日志  
						 SysDic 数据字典  
						 User 系统账户  

Common：公用类库  
Client：主客户端（包含管理中心）  
CustomerPlugin：客户管理插件  
ERPPlugin：库存管理插件  
FinancePlugin：财务中心插件  
HRPlugin：人事管理插件  

# 扩展插件介绍

FixedAssetsPlugin：固定资产管理插件  
LiveChartsTestPlugin：图表工具测试插件  
DBs/SaleDBModels ： 销售数据实体  
SaleOrder 客户销售订单表  
SalePlugin：销售订单中心插件（进行中）  



