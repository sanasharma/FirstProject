using System;
using System.Collections.Generic;
using Models.Model;

namespace Models.ViewModel.User
{
    public class UserDetailViewModel : Base.BaseViewModel
    {
        public UserDetailViewModel() {
            this.Email = "";
            this.Password = "";
            this.Account = "";
            this.Name = "";
            this.FindGroupID = "";
            this.GroupItems = new List<GroupItem>();
        }
        public int ID { set; get; }
        public string Email { set; get; }
        public string Password { set; get; }
        public string Account { set; get; }
        public string Name { set; get; }
        public string FindGroupID { set; get; }
        public bool IsSuper { set; get; }
        public bool IsLock { set; get; }
        public DateTime LastLoginDate { set; get; }
        public DateTime CreateDate { set; get; }

        public List<MenuAuth> AuthList { set; get; }
        public List<GroupItem> GroupItems { set; get; }
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

    public class GroupItem
    {
        public string GroupID { get; set; }
        public string GroupName { get; set; }
        public string GroupDesc { get; set; }
    }
}