using Panuon.UI.Silver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public abstract class BaseMainWindow : WindowX
    {
        public abstract void ShowLeftMenu(bool _show);
        public abstract void ShowTopMenu(bool _show);
        public abstract void ReLoadCurrTopMenu();
        public abstract void SetFrameSource(string _s);
    }
}
