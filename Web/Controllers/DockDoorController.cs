using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using System.Web.Mvc;
using EPC.Controllers.Base;
using DataAccess;
using Models.Base;
using Models.ViewModel.DockDoor;
using Tool;
using static Models.ViewModel.DockDoor.DockDoorDetailViewModel;
using System.Linq;

namespace EPC.Controllers
{
    [MyAuthorize(MenuNo = "0201")]
    public class DockDoorController : Base.MyController
    {

        public ActionResult List(string DockDoorID = "", int PageIndex = 1, int PageSize = 10)
        {
            try
            {
                //初始化物件
                var m = new DockDoorListViewModel();
                m.Authority = Tool.GetPageAuthority();
                m.Parameters = new Models.ViewModel.DockDoor.QueryParameter();
                m.Pages = new Pages();

                //設定參數
                m.Parameters.DockDoorID = DockDoorID;
                m.Pages.PageIndex = PageIndex;
                m.Pages.PageSize = PageSize;

                //清單資料
                var dt = DockDoorDataAccess.GetDockDoorList(DockDoorID, null, null, null, m.Pages);
                m.List = Util.ToList<ListItem>(dt);

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
                var m = new DockDoorDetailViewModel();



                //取得資料
                var dt = DockDoorDataAccess.GetDockDoorList(id, null, null, null, new Pages());
                var list = Util.ToList<DockDoorDetailViewModel>(dt);
                m = list[0];

                //載入Tag白名單規則
                DataTable dtFilter = FilterDataAccess.GetFilterList(null);
                m.FilterList = Util.ToList<Models.Model.Filter>(dtFilter);

                return View("Detail", m);
            }
            catch
            {
                return RedirectToAction("Error", "Home");
            }
        }

        public ActionResult SpecialTruckDetails(string Truck_No = "", string dockDoorID="",string Status="I")
        {
            try
            {

                var mlist = new DockDoorDetailViewModel();

                mlist.Parameters = new DockDoorDetailViewModel.QueryParameter();

                mlist.Truck_ID = Truck_No;

                mlist.Parameters.TRUCK_NO = "";

                mlist.DockDoorID = dockDoorID;

                if(dockDoorID!="" && Status!="")
                {
                    Session["DockDoorID"] = dockDoorID;
                    Session["Status"] = Status;
                }

                var dt = ApiDataAccess.GetSpecial_TruckDetails();
                    //  m = dt[0];
                    mlist.List = Util.ToList<RFID_TRUCK>(dt);
                    int i = 1;
                    foreach (var id in mlist.List)
                    {
                        id.Truck_ID = "T" + i++;
                    }
                
                if((Truck_No != ""))
                {
                    var dtt = ApiDataAccess.GetSpecialTruckFilteredDetails(Truck_No);
                    mlist.TruckFilterList = Util.ToList<RFID_TRUCK_Filter>(dtt);
                }
             

                return View("SpecialTruckDetails", mlist);

            }
            catch (Exception ex)
            {
                var mes = ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        //public ActionResult SpecialTruck(string dockdoorId)
        //{
        //    try
        //    {
        //        var mlist = new DockDoorDetailViewModel();

        //        mlist.Parameters = new DockDoorDetailViewModel.QueryParameter();

        //        mlist.Truck_ID = "";

        //        mlist.DockDoorID = dockdoorId;

        //        mlist.Parameters.TRUCK_NO = string.Empty ;

        //        var dt = ApiDataAccess.GetSpecialTruckDetails();
        //            //  m = dt[0];
        //            mlist.List = Util.ToList<RFID_TRUCK>(dt);
        //            int i = 1;
        //            foreach (var id in mlist.List)
        //            {
        //                id.Truck_ID ="T"+i++;
        //            }
             

        //        return View("SpecialTruckDetails", mlist);
                
             
               
        //    }
        //    catch(Exception ex)
        //    {
        //        var mes = ex.Message;
        //        return RedirectToAction("Error", "Home");
        //    }
        //}

        //public ActionResult FilterSpecialTruck(string TruckNo)
        //{
        //    var m = new DockDoorDetailViewModel();

        //    m.Parameters = new DockDoorDetailViewModel.QueryParameter();

        //    m.Parameters.TRUCK_NO = TruckNo;

        //    m.Truck_ID = "";

        //    var dt = ApiDataAccess.GetSpecialTruckDetails();
        //    //  m = dt[0];
        //    m.List = Util.ToList<RFID_TRUCK>(dt);
        //    int i = 1;
        //    foreach (var id in m.List)
        //    {
        //        id.Truck_ID ="T"+i++;
        //    }



        //    var dtt = ApiDataAccess.GetSpecialTruckFilteredDetails(TruckNo);
        //        m.TruckFilterList = Util.ToList<RFID_TRUCK_Filter>(dtt);
        //        return View("SpecialTruckDetails", m);

           

        //}

        [HttpPost]
        public JsonResult saveSpecialTruckDetails(string truckId="", string status="",string dockDoorID="")
        {
            Result r = new Result();
            if (status == "" && dockDoorID=="")
            {
                 dockDoorID = Session["DockDoorID"].ToString();
                status = Session["Status"].ToString();
            }
          
            if (status == "I")
            {
                    try
                    {
                        SpecialTruckDataAccess.SaveDockDoorInfo(truckId, status, dockDoorID);
                        r.Set(ResultCode.Success, "作業成功");
                    }
                    catch (Exception e)
                    {
                        r.Set(ResultCode.Error, e.Message);
                    }
              
            }
            else
            {
                try
                {
                    SpecialTruckDataAccess.SaveDockDoorInfo(truckId, status, dockDoorID);
                    r.Set(ResultCode.Success, "作業成功");
                }
                catch (Exception e)
                {
                    r.Set(ResultCode.Error, e.Message);
                }
            }

            return Json(r, JsonRequestBehavior.AllowGet);

        }

        public ActionResult Add()
        {
            try
            {
                //初始化物件
                var m = new DockDoorDetailViewModel();

                //載入Tag白名單規則
                DataTable dtFilter = FilterDataAccess.GetFilterList(null);
                m.FilterList = Util.ToList<Models.Model.Filter>(dtFilter);

                m.FilterCode = "";
                m.Mask = 100;
                m.CreateTime = DateTime.Now;
                m.UpdateTime = DateTime.Now;

                return View("Detail", m);
            }
            catch
            {
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public JsonResult Save(DockDoorDetailViewModel m)
        {
            Result r = new Result();

            try
            {
                DockDoorDataAccess.SaveDockDoorInfo(m);
                r.Set(ResultCode.Success, "作業成功");
            }
            catch (Exception e)
            {
                r.Set(ResultCode.Error, e.Message);
            }

            return Json(r, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ClearAlarm(string id)
        {
            Result r = new Result();

            try
            {
                ApiDataAccess.ClearAlarm(id);
                r.Set(ResultCode.Success, "作業成功");
            }
            catch (Exception e)
            {
                r.Set(ResultCode.Error, e.Message);
            }

            return Json(r, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateTruckStatus(string ip)
        {
            string strContainerStatus = "", strMsg = "";

            try
            {
                strMsg = DockDoorDataAccess.UpdateTruckStatus(ip, out strContainerStatus);
            }
            catch (Exception e)
            {
                strMsg = e.Message;
            }

            //回傳格式
            var obj = new
            {
                Status = strContainerStatus,
                Msg = strMsg
            };

            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult OpenSource(string ip)
        {
            Result r = new Result();

            try
            {
                //判斷裝置是否可通訊
                bool bPing = Util.PingIP(ip);
                if (bPing)
                {
                    //異步發出SSH指令
                    var task1 = Task.Factory.StartNew(() =>
                    {
                        //遠端SSH控制器指令(先關在開)
                        SSH ssh = new SSH(ip, 22, "pi", "raspberry");
                        ssh.RunCommand("sudo /etc/init.d/inno stop");
                        Thread.Sleep(500);
                        ssh.RunCommand("sudo /etc/init.d/inno start");
                    });

                    DockDoorDataAccess.SaveDockDoorInfo_InnoapStatus(ip, "O");
                    r.Set(ResultCode.Success, "作業成功");
                }
                else
                {
                    throw new Exception(ip + "無法通訊，開啟作業失敗");
                }
            }
            catch (Exception e)
            {
                r.Set(ResultCode.Error, e.Message);
            }

            return Json(r, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult CloseSource(string ip)
        {
            Result r = new Result();

            try
            {
                //判斷裝置是否可通訊
                bool bPing = Util.PingIP(ip);
                if (bPing)
                {
                    //異步發出SSH指令
                    var task1 = Task.Factory.StartNew(() =>
                    {
                        //遠端SSH控制器指令
                        SSH ssh = new SSH(ip, 22, "pi", "raspberry");
                        ssh.RunCommand("sudo /etc/init.d/inno stop");
                    });

                    DockDoorDataAccess.SaveDockDoorInfo_InnoapStatus(ip, "C");
                    r.Set(ResultCode.Success, "作業成功");
                }
                else
                {
                    throw new Exception(ip + "無法通訊，關閉作業失敗");
                }
            }
            catch (Exception e)
            {
                r.Set(ResultCode.Error, e.Message);
            }

            return Json(r, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Export(DockDoorListViewModel m)
        {
            //分頁物件，設定單頁筆數為最大值
            Pages pages = new Pages();
            pages.PageSize = int.MaxValue;

            //取回資料
            DataTable dt = DockDoorDataAccess.GetDockDoorList(null, m.Parameters.DockDoorID, null, null, pages);

            //轉為二進位資料流
            var numList = new List<int>();
            MemoryStream ms = NPOITools.RenderDataTableToExcel(dt, numList) as MemoryStream;

            return File(ms.ToArray(), "application/vnd.ms-excel");
        }
    }
}