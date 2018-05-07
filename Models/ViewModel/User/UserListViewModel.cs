using System;
using System.Collections.Generic;
using Models.Model;
using Models.Base;

namespace Models.ViewModel.User
{
    public class UserListViewModel : Base.BaseViewModel
    {
        public QueryParameter Parameters { set; get; }
        public List<ListItem> List { set; get; }
        public Auth Authority { set; get; }
        public Pages Pages { set; get; }
    }

    public class QueryParameter
    {
        public string Email { set; get; }
        public string Account { set; get; }
        public string Name { set; get; }
    }

    public class ListItem
    {
        public int ID { set; get; }
        public string Email { set; get; }
        public string Account { set; get; }
        public string Name { set; get; }
        public bool IsSuper { set; get; }
        public bool IsLock { set; get; }
        public DateTime CreateDate { set; get; }
    }
}