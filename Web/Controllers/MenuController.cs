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
using Models.ViewModel.Menu;
using Newtonsoft.Json;
using Tool;

namespace EPC.Controllers
{
    [MyAuthorize(MenuNo = "0102")]
    public class MenuController : Base.MyController
    {
        public ActionResult List()
        {
            try
            {
                //初始化物件
                var m = new MenuListViewModel();
                m.Authority = Tool.GetPageAuthority();

                //所有選單資料
                var dtMenu = MenuDataAccess.GetAllMenuList(null);
                m.List = Util.ToList<ListItem>(dtMenu);

                return View(m);
            }
            catch
            {
                return RedirectToAction("Error", "Home");
            }
        }

        public ActionResult Add(string id = "")
        {
            try
            {
                //初始化物件
                var m = new MenuDetailViewModel();
                m.ParentMenuNo = id;
                m.Type = "";
                m.Enabled = true;

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
                //初始化物件
                var m = new MenuDetailViewModel();

                //取出資料
                var dt = MenuDataAccess.GetMenuList(null, id, null, null);
                var list = Util.ToList<MenuDetailViewModel>(dt);

                m = list[0];

                return View("Detail", m);
            }
            catch
            {
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public JsonResult Save(MenuDetailViewModel m)
        {
            Result r = new Result();

            try
            {
                MenuDataAccess.SaveMenuInfo(m);
                r.Set(ResultCode.Success, "作業成功");
            }
            catch (Exception e)
            {
                r.Set(ResultCode.Error, e.Message);
            }

            return Json(r, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Del(string id)
        {
            Result r = new Result();

            try
            {
                MenuDataAccess.DelMenu(id);
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