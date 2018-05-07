//using System.Web.Mvc;
//using EPC.Controllers.Base;
//using DataAccess;
//using Models.Base;
//using Models.ViewModel.SpecialTruck;
//using Tool;
//using System;
//using System.Linq;
//using System.Collections.Generic;
//using Models.Model;
//using Models.ViewModel.DockDoor;
//using static Models.ViewModel.DockDoor.DockDoorDetailViewModel;

//namespace EPC.Controllers
//{
//    [MyAuthorize(MenuNo = "0203")]
//    public class SpecialTruckController : Base.MyController
//    {
//        // GET: SpecialTruck
//        //public ActionResult SpecialTruckDetails(string dockdoorId,string Truck_ID="")
//        ////{
//        //    try
//        //    {

//        //        var mlist = new DockDoorDetailViewModel();

//        //        mlist.Parameters = new DockDoorDetailViewModel.QueryParameter();

//        //        mlist.Truck_ID = Truck_ID;

//        //        mlist.DockDoorID = dockdoorId;

//        //        mlist.Parameters.TRUCK_NO = string.Empty;

//        //        var dt = ApiDataAccess.GetSpecial_TruckDetails(Truck_ID);
//        //        //  m = dt[0];
//        //        mlist.List = Util.ToList<RFID_TRUCK>(dt);
//        //        int i = 1;
//        //        foreach (var id in mlist.List)
//        //        {
//        //            id.Truck_ID = "T" + i++;
//        //        }
               
//        //        return View("SpecialTruckDetails",mlist);

//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        var mes = ex.Message;
//        //        return RedirectToAction("Error", "Home");
//        //    }
//        //}

//        //public ActionResult FilterSpecialTruck(string TruckNo)
//        //{
//        //    var m = new DockDoorDetailViewModel();

//        //    m.Parameters = new DockDoorDetailViewModel.QueryParameter();

//        //    m.Parameters.TRUCK_NO = TruckNo;

//        //    m.Truck_ID = "";

//        //    var dt = ApiDataAccess.GetSpecialTruckDetails();
//        //    //  m = dt[0];
//        //    m.List = Util.ToList<RFID_TRUCK>(dt);
//        //    int i = 1;
//        //    foreach (var id in m.List)
//        //    {
//        //        id.Truck_ID = "T" + i++;
//        //    }



//        //    var dtt = ApiDataAccess.GetSpecialTruckFilteredDetails(TruckNo);
//        //    m.TruckFilterList = Util.ToList<RFID_TRUCK_Filter>(dtt);
//        //    return View("SpecialTruckDetails", m);



//        //}


//        //[HttpPost]
//        //public JsonResult Save(string truckId, string status, string dockdoorId)
//        //{
//        //    Result r = new Result();
//        //    if(status=="I")
//        //    {
//        //        if (dockdoorId != "" && dockdoorId != "null")
//        //        {
//        //            try
//        //            {
//        //                SpecialTruckDataAccess.SaveDockDoorInfo(truckId, status, dockdoorId);
//        //                r.Set(ResultCode.Success, "作業成功");
//        //            }
//        //            catch (Exception e)
//        //            {
//        //                r.Set(ResultCode.Error, e.Message);
//        //            }

//        //        }
//        //        else
//        //        {
//        //            r.Set(ResultCode.Error, "Please Choose Correct DockDoor");
//        //        }
//        //    }
//        //    else
//        //    {
//        //        try
//        //        {
//        //            SpecialTruckDataAccess.SaveDockDoorInfo(truckId, status, dockdoorId);
//        //            r.Set(ResultCode.Success, "作業成功");
//        //        }
//        //        catch (Exception e)
//        //        {
//        //            r.Set(ResultCode.Error, e.Message);
//        //        }
//        //    }
         
//        //    return Json(r, JsonRequestBehavior.AllowGet);

//        //}
//    }
//}