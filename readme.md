# 注意

项目现在正在开发中，当前可用功能：  
账户管理  
角色授权  
插件管理  

# WPF开发的管理系统（数据库MSSqlserver）

项目采用插件式方式开发,在开发时可以更好的业务分离,提高多人协作开发效率.  

# 项目中使用到的技术

NETFramework472  
Panuon.UI.Silver  
LiveCharts  
NPOI.Excel  
Hangfire  
NLog  
Newtonsoft.Json  
EntityFramework(Code First)  


开发环境：  
VS2019  
Sqlserver2019  

# 效果    

![image](https://github.com/straw-git/WPFManager/blob/master/效果图/登录.jpg)  
![image](https://github.com/straw-git/WPFManager/blob/master/效果图/选择插件.jpg)  
![image](https://github.com/straw-git/WPFManager/blob/master/效果图/账号管理.jpg)  


# 正在进行中（闲时开发中，尚不完善）

定时任务    



# 更新日志

220425更新：  
插件管理：将插件管理移入数据库操作，插件下载使用从网络端更新  
移除页面过多依赖，使插件间更加独立  
账号管理：系统登录账号管理及授权    
角色授权    

# 目录介绍
Client：程序入口，拥有登录、主页、主题设置窗口，主要功能是发现和解析插件数据  
CorePlugin：核心管理模块，仅包含管理员、系统公用数据、及权限的维护  


