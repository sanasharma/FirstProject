using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModel.Menu
{
    public class MenuDetailViewModel : Base.BaseViewModel
    {
        public string MenuNo { set; get; }
        public string MenuName { set; get; }
        public string MenuLink { set; get; }
        public string Type { set; get; }
        public string MenuDesc { set; get; }
        public int OrderID { set; get; }
        public bool Enabled { set; get; }
        public string MenuIco { set; get; }

        public string ParentMenuNo { set; get; }
    }
}
