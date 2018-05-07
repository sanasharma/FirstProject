using System;
using System.Data;
using System.IO;
using System.Collections.Generic;
using System.Web.Mvc;
using EPC.Controllers.Base;
using DataAccess;
using Models.Base;
using Models.ViewModel.TagLog;
using Tool;

namespace EPC.Controllers
{
    [MyAuthorize(MenuNo = "0202")]
    public class TagLogController : Base.MyController
    {
        public ActionResult List(string DockDoorID = "", string Status = "", int PageIndex = 1, int PageSize = 10)
        {
            try
            {
                //初始化物件
                var m = new TagLogListViewModel();
                m.Authority = Tool.GetPageAuthority();
                m.Parameters = new QueryParameter();
                m.Pages = new Pages();

                //設定參數
                m.Parameters.DockDoorID = DockDoorID;
                m.Parameters.Status = Status;
                m.Pages.PageIndex = PageIndex;
                m.Pages.PageSize = PageSize;

                //清單資料
                var dt = TagLogDataAccess.GetTagLogList(DockDoorID, Status, m.Pages);
                m.List = Util.ToList<ListItem>(dt);

                return View(m);
            }
            catch
            {
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public ActionResult Export(TagLogListViewModel m)
        {
            //分頁物件，設定單頁筆數為最大值
            Pages pages = new Pages();
            pages.PageSize = int.MaxValue;

            //取回資料
            DataTable dt = TagLogDataAccess.GetTagLogList(m.Parameters.DockDoorID, null, pages);

            //轉為二進位資料流
            var numList = new List<int>();
            MemoryStream ms = NPOITools.RenderDataTableToExcel(dt, numList) as MemoryStream;

            return File(ms.ToArray(), "application/vnd.ms-excel");
        }
    }
}