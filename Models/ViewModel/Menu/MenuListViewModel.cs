using System;
using System.Collections.Generic;
using Models.Model;
using Models.Base;

namespace Models.ViewModel.Menu
{
    public class MenuListViewModel : Base.BaseViewModel
    {
        public List<ListItem> List { set; get; }
        public Auth Authority { set; get; }
    }

    public class ListItem
    {
        public string MenuNo { set; get; }
        public string MenuName { set; get; }
        public string Type { set; get; }
        public string MenuDesc { set; get; }
        public int OrderID { set; get; }
        public bool Enabled { set; get; }
        public string MenuIco { set; get; }
    }
}