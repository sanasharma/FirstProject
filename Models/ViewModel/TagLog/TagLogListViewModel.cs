using System;
using System.Collections.Generic;
using Models.Model;
using Models.Base;

namespace Models.ViewModel.TagLog
{
    public class TagLogListViewModel : Base.BaseViewModel
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
        public int Mask { set; get; }
        public string Status { set; get; }
        public string Msg { set; get; }
        public DateTime DateTime { set; get; }
        public string Data { set; get; }
        public string PostData { set; get; }
    }

    public class QueryParameter
    {
        public string DockDoorID { set; get; }
        public string Status { set; get; }
    }
}