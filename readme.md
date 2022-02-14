# WPF开发的管理系统（数据库MSSqlserver）

项目采用插件式方式开发,在开发时可以更好的业务分离,提高多人协作开发效率.

# WPF中的样式收集已转移至  

[https://zhuanlan.zhihu.com/p/459008647](https://zhuanlan.zhihu.com/p/459008647)  

# 项目中使用到的技术

NETFramework472  
Panuon.UI.Silver  
LiveCharts  
EntityFramework CodeFirst  
NPOI.Excel  

开发环境：  
VS2019  
Sqlserver2019  

# 正在进行中（闲时开发中，尚不完善）

完善Client端  

# 更新日志

220212更新：V1.1-001  
Client为客户端,将其它功能分离  


# 如何新增插件  

1. 复制NewPlugin项目并重命名插件名称  
使用VS单独打开并重新命名  
需注意项目属性中的‘程序集名称’和‘默认命名空间’与新名称对应  
引用需要的dll  
2. 编辑项目根目录logo.jpg（标准尺寸宽高比例 2:1） 作为当前插件的logo （logo名称与PluginsInfo.cs中对应） 
3. 在Client/AutoUpdatePlugins.cs中将新建的插件dll路径加入到自动更新中（便于开发调试,不用经常手动更新）  

