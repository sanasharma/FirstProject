using System;
using System.Data;
using System.IO;
using System.Collections.Generic;
using System.Web.Mvc;
using EPC.Controllers.Base;
using DataAccess;
using Models.Base;
using Models.ViewModel.TagRule;
using Tool;

namespace EPC.Controllers
{
    [MyAuthorize(MenuNo = "0204")]
    public class TagRuleController : Base.MyController
    {
        public ActionResult List(int PageIndex = 1, int PageSize = 10)
        {
            try
            {
                //初始化物件
                var m = new TagRuleListViewModel();
                m.Authority = Tool.GetPageAuthority();

                //清單資料
                var dt = DockDoorDataAccess.GetFilterList(null);
                m.List = Util.ToList<Models.Model.Filter>(dt);

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
                //初始化物件
                var m = new TagRuleDetailViewModel();
                m.Action = "Add";

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
                var m = new TagRuleDetailViewModel();

                //取得系統參數資料
                var dt = DockDoorDataAccess.GetFilterList(id);
                var list = Util.ToList<TagRuleDetailViewModel>(dt);
                m = list[0];
                m.Action = "Edit";

                return View("Detail", m);
            }
            catch
            {
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public JsonResult Save(TagRuleDetailViewModel m)
        {
            Result r = new Result();

            try
            {
                DockDoorDataAccess.SaveFilter(m);
                r.Set(ResultCode.Success, "作業成功");
            }
            catch (Exception e)
            {
                r.Set(ResultCode.Error, e.Message);
            }

            return Json(r, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Remove(string id)
        {
            Result r = new Result();

            try
            {
                DockDoorDataAccess.RemoveFilter(id);
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