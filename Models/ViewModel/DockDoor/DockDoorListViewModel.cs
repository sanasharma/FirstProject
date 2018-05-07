using System;
using System.Collections.Generic;
using Models.Model;
using Models.Base;

namespace Models.ViewModel.DockDoor
{
    public class DockDoorListViewModel : Base.BaseViewModel
    {
        public QueryParameter Parameters { set; get; }
        public List<ListItem> List { set; get; }
        public Auth Authority { set; get; }
        public Pages Pages { set; get; }
    }

    public class ListItem
    {
        public string DockDoorID { set; get; }
        public string IP { set; get; }
        public string Locate { set; get; }
        public string FilterName { set; get; }
        public string Alarm { set; get; }
        public string ContainerID { set; get; }
        public string ContainerStatus { set; get; }
        public bool Flag { set; get; }
        public string InnoapStatus { set; get; }

    }

    public class QueryParameter
    {
        public string DockDoorID { set; get; }
    }
}