using System;
using System.IO;
using System.Collections.Generic;
using System.Web.Mvc;
using EPC.Controllers.Base;
using DataAccess;
using Models.Base;
using Models.ViewModel.SysLog;
using Tool;

namespace EPC.Controllers
{
    [MyAuthorize(MenuNo = "0105")]
    public class SysLogController : Base.MyController
    {
        public ActionResult List(string Account = "", string IP = "", int PageIndex = 1, int PageSize = 10)
        {
            try
            {
                //初始化物件
                var m = new SysLogListViewModel();
                m.Authority = Tool.GetPageAuthority();
                m.Parameters = new QueryParameter();
                m.Pages = new Pages();

                //設定參數
                m.Parameters.Account = Account;
                m.Parameters.IP = IP;
                m.Pages.PageIndex = PageIndex;
                m.Pages.PageSize = PageSize;

                //清單資料
                var dtLog = SysLogDataAccess.GetUserLoginLog(m.Parameters.Account, m.Parameters.IP, m.Pages);
                m.List = Util.ToList<ListItem>(dtLog);

                return View(m);
            }
            catch
            {
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public ActionResult Export(SysLogListViewModel m)
        {
            //分頁物件，設定單頁筆數為最大值
            Pages pages = new Pages();
            pages.PageSize = int.MaxValue;

            //取回資料
            var dt = SysLogDataAccess.GetUserLoginLog(m.Parameters.Account, m.Parameters.IP, pages);

            //轉為二進位資料流
            var numList = new List<int>();
            MemoryStream ms = NPOITools.RenderDataTableToExcel(dt, numList) as MemoryStream;

            return File(ms.ToArray(), "application/vnd.ms-excel");
        }
    }
}