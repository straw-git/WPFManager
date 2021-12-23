using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities
{
    public class MainWindowTagInfo
    {
        public DBModels.Sys.User CurrUser { get; set; }
        public Dictionary<string, Dictionary<BaseMenuInfo, List<MenuItemModel>>> Dic;
    }
}
