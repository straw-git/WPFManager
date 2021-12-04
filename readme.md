# WPF开发的管理系统（数据库MSSqlserver）
MSSqlserver数据库的增删改查  
使用LiveCharts展示图表  
使用NPOI导出Excel  
使用Panuon.UI.Silver搭建漂亮的UI页面

# 目录介绍
Common：公用类库  
Client：主客户端（包含管理中心）  
CustomerPlugin：客户管理插件  
ERPPlugin：库存管理插件  
FinancePlugin：财务中心插件  
FixedAssetsPlugin：固定资产管理插件  
HRPlugin：人事管理插件  
LiveChartsTestPlugin：图表工具测试插件  
SalePlugin：销售订单中心插件（未完成）  

# 基础模块包括：

账号管理  
权限管理  
数据字典  

# 插件模块包括：

人事管理（人员信息、工资、保险）  
财务管理（收款账号、工资发放）  
固定资产管理  
库存管理  
客户会员管理  

# 示例文章

[客户管理插件文章](https://zhuanlan.zhihu.com/p/439497177)    
[搭建及扩展文章](https://zhuanlan.zhihu.com/p/428356007)  
[基础功能示例文章](https://zhuanlan.zhihu.com/p/431962796)

项目中使用到的技术：

NETFramework472  
Panuon.UI.Silver  
LiveCharts  
EntityFramework CodeFirst  
NPOI.Excel  

开发环境：  
VS2019+Sqlserver2019

项目采用插件式方式开发,在开发时可以更好的业务分离,提高多人协作开发效率.
