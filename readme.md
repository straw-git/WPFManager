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

编辑主题功能  
编辑页面样式  
删除部分插件功能    

# 更新日志

220212更新：V1.1-001
精简功能，完善插件标记，添加账套，去除权限


# 如何新增插件  

1. 复制DemoPlugin项目并重命名插件名称  
需注意项目属性中的‘程序集名称’和‘默认命名空间’与新名称对应  
2. 删除Pages文件夹下的文件夹（仅删除示例文件夹即可）或 将文件夹内的MenuInfo.cs删除  
3. 编辑项目根目录logo.jpg（标准尺寸宽高比例 2:1） 作为当前插件的logo （logo名称与PluginsInfo.cs中对应） 
4. 在Client/AutoUpdatePlugins.cs中将新建的插件dll路径加入到自动更新中（便于开发调试,不用经常手动更新）  

