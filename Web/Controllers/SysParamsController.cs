using System;
using System.IO;
using System.Collections.Generic;
using System.Web.Mvc;
using EPC.Controllers.Base;
using DataAccess;
using Models.Base;
using Models.ViewModel.SysParams;
using System.Data;
using Tool;


namespace EPC.Controllers
{
    [MyAuthorize(MenuNo = "0104")]
    public class SysParamsController : Base.MyController
    {
        public ActionResult List(string ParaCode = "", string ParaDesc = "", int PageIndex = 1, int PageSize = 10)
        {
            try
            {
                //初始化物件
                var m = new SysParamsListViewModel();
                m.Authority = Tool.GetPageAuthority();
                m.Parameters = new QueryParameter();
                m.Pages = new Pages();

                //設定參數
                m.Parameters.ParaCode = ParaCode;
                m.Parameters.ParaDesc = ParaDesc;
                m.Pages.PageIndex = PageIndex;
                m.Pages.PageSize = PageSize;

                //清單資料
                DataTable dtList = SysParamsDataAccess.GetSysParams(m.Parameters.ParaCode, m.Parameters.ParaDesc, m.Pages);
                m.List = Util.ToList<ListItem>(dtList);

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
                var m = new SysParamsDetailViewModel();

                //取得系統參數資料
                var dtParams = SysParamsDataAccess.GetSysParams(id, null, new Pages());
                var listParams = Util.ToList<SysParamsDetailViewModel>(dtParams);
                m = listParams[0];

                m.Action = "Edit";

                return View("Detail", m);
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
                var m = new SysParamsDetailViewModel();
                m.Action = "Add";

                return View("Detail", m);
            }
            catch
            {
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public JsonResult Save(SysParamsDetailViewModel m)
        {
            Result r = new Result();

            try
            {
                SysParamsDataAccess.SaveSysParamsInfo(m);
                r.Set(ResultCode.Success, "作業成功");
            }
            catch (Exception e)
            {
                r.Set(ResultCode.Error, e.Message);
            }

            return Json(r, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Export(SysParamsListViewModel m)
        {
            //分頁物件，設定單頁筆數為最大值
            Pages pages = new Pages();
            pages.PageSize = int.MaxValue;

            //清單資料
            DataTable dtList = SysParamsDataAccess.GetSysParams(m.Parameters.ParaCode, m.Parameters.ParaDesc, m.Pages);

            //轉為二進位資料流
            var numList = new List<int>();
            MemoryStream ms = NPOITools.RenderDataTableToExcel(dtList, numList) as MemoryStream;

            return File(ms.ToArray(), "application/vnd.ms-excel");
        }
    }
}