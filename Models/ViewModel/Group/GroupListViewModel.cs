using System;
using System.Collections.Generic;
using Models.Model;
using Models.Base;

namespace Models.ViewModel.Group
{
    public class GroupListViewModel : Base.BaseViewModel
    {
        public QueryParameter Parameters { set; get; }
        public List<ListItem> List { set; get; }
        public Auth Authority { set; get; }
        public Pages Pages { set; get; }
    }

    public class ListItem
    {
        public string GroupID { set; get; }
        public string GroupName { set; get; }
        public string GroupDesc { set; get; }
    }

    public class QueryParameter
    {
        public string GroupName { set; get; }
    }
}