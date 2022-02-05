using System.Collections.Generic;

namespace Common.Entities
{
    public class MenuItemButtonModel 
    {
        public string Name { get; set; }
        public string Content { get; set; }
    }

    public class MenuItemModel
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string ParentCode { get; set; }
        public string PluginCode { get; set; }
        public string Url { get; set; }
        public int Order { get; set; }
    }
}
