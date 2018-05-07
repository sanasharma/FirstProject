using System;
using System.Collections.Generic;
using Models.Model;

namespace Models.ViewModel.Group
{
    public class GroupDetailViewModel : Base.BaseViewModel
    {
        public GroupDetailViewModel() {
            this.GroupID = "";
            this.GroupName = "";
            this.GroupDesc = "";
            this.FindUserAccount = "";
            this.GroupUserItems = new List<GroupUserItem>();
        }
        public string GroupID { set; get; }
        public string GroupName { set; get; }
        public string GroupDesc { set; get; }
        public string FindUserAccount { set; get; }
        public List<MenuAuth> AuthList { set; get; }
        public List<GroupUserItem> GroupUserItems { set; get; }
    }

    public class MenuAuth
    {
        public string MenuNo { get; set; }
        public string MenuName { get; set; }
        public string MenuIco { get; set; }
        public int OrderID { get; set; }
        public bool Add { get; set; }
        public bool Edit { get; set; }
        public bool Del { get; set; }
        public bool Query { get; set; }
        public bool Audit { get; set; }
        public bool Print { get; set; }
        public bool Export { get; set; }
        public bool Import { get; set; }
        public bool Admin { get; set; }
    }

    public class GroupUserItem
    {
        public int ID { get; set; }
        public string Account { get; set; }
        public string Name { get; set; }
    }
}