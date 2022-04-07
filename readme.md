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
EntityFramework CodeFirst  
NPOI.Excel  

开发环境：  
VS2019  
Sqlserver2019  

# 代码目录  

![image](https://github.com/straw-git/WPFManager/blob/master/效果图/目录.jpg)  


# 正在进行中（闲时开发中，尚不完善）

分页用户控件  



# 更新日志

220407更新：  
插件管理：将插件管理移入数据库操作，插件下载使用从网络端更新  
移除页面过多依赖，使插件间更加独立  
账号管理：系统登录账号管理及授权    
角色授权    


220212更新：V1.1-001  
完善客户端及插件端功能  
添加账套管理来展现插件信息  
添加窗口管理器管理多窗口  
各插件端可独立编辑测试  
各插件端数据管理独立  
账套动态读取插件信息  
可根据账套选择打开多个主窗口  

# 目录介绍
Client：程序入口，拥有登录、主页、主题设置窗口，主要功能是发现和解析插件数据  
CorePlugin：核心管理模块，仅包含管理员、系统公用数据、及权限的维护  

# 如何新增插件  


# 如何独立运行插件模块

1. 将插件项目属性中的输出类型设置为‘Windows应用程序’  
2. 将插件项目APP.config属性中生成操作设置为‘ApplicationDefinition’  
3. 将插件项目设置为启动项  

# WPF中的样式收集已转移至  


[知乎文章 WPF样式收集](https://zhuanlan.zhihu.com/p/459008647)  

[源码  GITHUB](https://github.com/straw-git/WPFStyles)  
