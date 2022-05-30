
# WPF开发的管理系统（数据库MSSqlserver）

项目采用插件式方式开发,在开发时可以更好的业务分离,提高多人协作开发效率.  

# 项目中使用到的技术

NETFramework472  
Panuon.UI.Silver  
LiveCharts  
NPOI.Excel  
NLog  
Newtonsoft.Json  
EntityFramework(Code First)  

开发环境：  
VS2019  
Sqlserver2019  


# 正在进行中（闲时开发中，尚不完善）

新功能开发  
插件管理  

# 功能截图

登录  
![image](https://raw.githubusercontent.com/straw-git/WPFManager/master/%E6%95%88%E6%9E%9C%E5%9B%BE/%E7%99%BB%E5%BD%95.jpg)  
选择插件  
![image]( https://github.com/straw-git/WPFManager/blob/master/%E6%95%88%E6%9E%9C%E5%9B%BE/选择插件.jpg)  
主页  
![image]( https://github.com/straw-git/WPFManager/blob/master/%E6%95%88%E6%9E%9C%E5%9B%BE/主页.jpg)  
人员管理  
![image]( https://github.com/straw-git/WPFManager/blob/master/%E6%95%88%E6%9E%9C%E5%9B%BE/人员管理.jpg)  
插件管理  
![image]( https://github.com/straw-git/WPFManager/blob/master/%E6%95%88%E6%9E%9C%E5%9B%BE/插件管理.jpg)  
邮件  
![image]( https://github.com/straw-git/WPFManager/blob/master/%E6%95%88%E6%9E%9C%E5%9B%BE/邮件.jpg)  

# 更新日志

220530更新：
登录功能（含插件选择）  
主页功能（邮件、皮肤、设置）
管理中心（人员管理、角色授权、插件管理、职位管理）  

# 目录介绍
Client：程序入口，拥有登录、主页、主题设置窗口，主要功能是发现和解析插件数据  
CorePlugin：核心管理模块，仅包含管理员、系统公用数据、及权限的维护  
CoreDBModels：基础数据模型（人员、职位、邮件、权限、日志、设置）（EF 数据模型）  

