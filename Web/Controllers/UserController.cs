using System;
using System.IO;
using System.Text;
using System.Web.Mvc;
using System.Data;
using System.Collections.Generic;
using EPC.Controllers.Base;
using DataAccess;
using Models.Base;
using Models.Model;
using Models.ViewModel.User;
using Newtonsoft.Json;
using Tool;

namespace EPC.Controllers
{
    [MyAuthorize(MenuNo = "0101")]
    public class UserController : Base.MyController
    {
        public ActionResult List(string Account = "", string Name = "", string Email = "", int PageIndex = 1, int PageSize = 10)
        {
            try
            {
                //初始化物件
                var m = new UserListViewModel();
                m.Authority = Tool.GetPageAuthority();
                m.Parameters = new QueryParameter();
                m.Pages = new Pages();

                //設定參數
                m.Parameters.Account = Account;
                m.Parameters.Email = Email;
                m.Parameters.Name = Name;
                m.Pages.PageIndex = PageIndex;
                m.Pages.PageSize = PageSize;

                //清單資料
                DataTable dtList = UserDataAccess.GetUserList(null, m.Parameters.Account, null, m.Parameters.Email, m.Parameters.Name, m.Pages);
                m.List = Util.ToList<ListItem>(dtList);

                return View(m);
            }
            catch
            {
                return RedirectToAction("Error", "Home");
            }
        }

        public ActionResult Add()
        {
            try
            {
                UserDetailViewModel m = new UserDetailViewModel();
                m.CreateDate = DateTime.Now;

                //取得排序過的選單清單
                var dtMenu = UserDataAccess.GetUserMenuAuth("0");
                m.AuthList = Util.ToList<MenuAuth>(dtMenu);

                return View("Detail", m);
            }
            catch
            {
                return RedirectToAction("Error", "Home");
            }
        }

        public ActionResult Edit(string id)
        {
            try
            {
                UserDetailViewModel m = new UserDetailViewModel();

                //取得使用者資料
                var dtList = UserDataAccess.GetUserList(null, id, null, null, null, new Pages());
                var list = Util.ToList<UserDetailViewModel>(dtList); //清單資料
                m = list[0];

                //取得排序過的選單清單
                var dtMenu = UserDataAccess.GetUserMenuAuth(id);
                m.AuthList = Util.ToList<MenuAuth>(dtMenu);

                //取得使用者所屬的群組清單
                var dtGroup = GroupDataAccess.GetUserGroup(id);
                m.GroupItems = Util.ToList<GroupItem>(dtGroup);

                return View("Detail", m);
            }
            catch
            {
                return RedirectToAction("Error", "Home");
            }
        }

        public ActionResult GetGroup(string id)
        {
            var dt = GroupDataAccess.GetGroupList(id, null, new Pages());
            var list = Util.ToList<GroupItem>(dt);

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Change(string id)
        {
            return Redirect("/Home/Login/" + id + "?type=Simulate");
        }

        [HttpPost]
        public JsonResult Save(UserDetailViewModel m)
        {
            Result r = new Result();

            try
            {
                UserDataAccess.SaveUserInfo(m);
                r.Set(ResultCode.Success, "作業成功");
            }
            catch (Exception e)
            {
                r.Set(ResultCode.Error, e.Message);
            }

            return Json(r, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Export(UserListViewModel m)
        {
            //分頁物件，設定單頁筆數為最大值
            Pages pages = new Pages();
            pages.PageSize = int.MaxValue;

            //取回資料
            DataTable dt = UserDataAccess.GetUserList(null, m.Parameters.Account, null, m.Parameters.Email, m.Parameters.Name, pages);

            //處理資料
            dt.Columns.Remove("Password");

            //轉為二進位資料流
            var numList = new List<int>();
            MemoryStream ms = NPOITools.RenderDataTableToExcel(dt, numList) as MemoryStream;

            return File(ms.ToArray(), "application/vnd.ms-excel");
        }

        [HttpPost]
        public ActionResult Resetpwd(string id)
        {
            Result r = new Result();

            try
            {
                UserDataAccess.SetUserDefaultPassword(id);
                r.Set(ResultCode.Success, "作業成功");
            }
            catch (Exception e)
            {
                r.Set(ResultCode.Error, e.Message);
            }

            return Json(r, JsonRequestBehavior.AllowGet);
        }
    }
}