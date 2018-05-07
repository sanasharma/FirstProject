using System;
using System.IO;
using System.Collections.Generic;
using System.Web.Mvc;
using EPC.Controllers.Base;
using DataAccess;
using Models.Base;
using Models.ViewModel.Group;
using Tool;

namespace EPC.Controllers
{
    [MyAuthorize(MenuNo = "0103")]
    public class GroupController : Base.MyController
    {
        public ActionResult List(string GroupName = "", int PageIndex = 1, int PageSize = 10)
        {
            try
            {
                //初始化物件
                var m = new GroupListViewModel();
                m.Authority = Tool.GetPageAuthority();
                m.Parameters = new QueryParameter();
                m.Pages = new Pages();

                //設定參數
                m.Parameters.GroupName = GroupName;
                m.Pages.PageIndex = PageIndex;
                m.Pages.PageSize = PageSize;

                //清單資料
                var dtGroup = GroupDataAccess.GetGroupList(null, m.Parameters.GroupName, m.Pages);
                m.List = Util.ToList<ListItem>(dtGroup);

                return View(m);
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
                //初始化物件
                var m = new GroupDetailViewModel();

                //取得群組資料
                var dtGroup = GroupDataAccess.GetGroupList(id, null, new Pages());
                var listGroup = Util.ToList<GroupDetailViewModel>(dtGroup);
                m = listGroup[0];

                //取得排序過的選單清單
                var dtMenu = GroupDataAccess.GetGroupMenuAuth(id);
                m.AuthList = Util.ToList<MenuAuth>(dtMenu);

                //取得群組底下的使用者清單
                var dtGroupUser = GroupDataAccess.GetGroupUser(id);
                m.GroupUserItems = Util.ToList<GroupUserItem>(dtGroupUser);

                return View("Detail", m);
            }
            catch
            {
                return RedirectToAction("Error", "Home");
            }
        }

        public ActionResult Add(string id)
        {
            try
            {
                //初始化物件
                var m = new GroupDetailViewModel();

                //取得排序過的選單清單
                var dtMenu = GroupDataAccess.GetGroupMenuAuth("0");
                m.AuthList = Util.ToList<MenuAuth>(dtMenu);

                return View("Detail", m);
            }
            catch
            {
                return RedirectToAction("Error", "Home");
            }
        }

        public ActionResult GetUser(string id)
        {
            var dt = UserDataAccess.GetUserList(null, id, null, null, null, new Pages());
            var list = Util.ToList<GroupUserItem>(dt);

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Save(GroupDetailViewModel m)
        {
            Result r = new Result();

            try
            {
                GroupDataAccess.SaveGroupInfo(m);
                r.Set(ResultCode.Success, "作業成功");
            }
            catch (Exception e)
            {
                r.Set(ResultCode.Error, e.Message);
            }

            return Json(r, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Export(GroupListViewModel m)
        {
            //分頁物件，設定單頁筆數為最大值
            Pages pages = new Pages();
            pages.PageSize = int.MaxValue;

            //取回資料
            var dt = GroupDataAccess.GetGroupList(null, m.Parameters.GroupName, pages);

            //轉為二進位資料流
            var numList = new List<int>();
            MemoryStream ms = NPOITools.RenderDataTableToExcel(dt, numList) as MemoryStream;

            return File(ms.ToArray(), "application/vnd.ms-excel");
        }

        [HttpPost]
        public ActionResult Remove(string id)
        {
            Result r = new Result();

            try
            {
                GroupDataAccess.RemoveGroup(id);
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