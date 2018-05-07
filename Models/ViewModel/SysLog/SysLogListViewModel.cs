using System;
using System.Collections.Generic;
using Models.Model;
using Models.Base;

namespace Models.ViewModel.SysLog
{
    public class SysLogListViewModel : Base.BaseViewModel
    {
        public QueryParameter Parameters { set; get; }
        public List<ListItem> List { set; get; }
        public Auth Authority { set; get; }
        public Pages Pages { set; get; }
    }

    public class QueryParameter
    {
        public string Account { set; get; }
        public string IP { set; get; }
    }

    public class ListItem
    {
        public int ID { set; get; }
        public string Account { set; get; }
        public string IP { set; get; }
        public DateTime LoginDate { set; get; }
    }
}