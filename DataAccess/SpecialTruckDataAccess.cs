using Models.Base;
using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
  public  class SpecialTruckDataAccess
    {
        public static DataTable GetSpecialTruckDockDoorList()
        {
            //開啟資料庫存取物件
            using (var db = new DataBase.DataBase(Definition.Conn))
            {
                //判斷Status成功或失敗
                DataTable dt = null;
               db.StrSQL = "SELECT DockDoorID,ContainerID FROM Innolux_DockDoor WHERE 1=1";
               dt=  db.ExecuteDataTable();
                return dt;
            }
        }

        public static void SaveDockDoorInfo(string truckID, string Status, string dockdoorID)
        {

            string O_Status = null;
            DataTable dt = null;
            DataTable dt1 = null;
            string IP = null;
            string strNum, strTotal;
            string cid = string.Empty;
            string cstatus = string.Empty;
            Result r = new Result();
            string ASN = null;
            string DN = null;
            string CaptionPanelIP = null;

            //Save in ORACLE
            using (var db = new DataBase.OracleDataBase(Definition.WMSConn))
                    {
               
                db.StrSQL = "SELECT DN_NO FROM WMS_RFID_CONTAINER_CHECK_V WHERE CONTAINER_NO='" + truckID + "'";

                dt1 = db.ExecuteDataTable();

                if (dt1.DefaultView.Count > 0)
                {
                    DataRow DN_dr = dt1.Rows[0];

                    DN = DN_dr["DN_NO"].ToString();
                }

            }

            using (var db = new DataBase.DataBase(Definition.Conn,true))
            {
                try
                {
                    db.StrSQL = "SELECT ASN_NO FROM WMS_RFID_TRUCK WHERE TRUCK_NO='" + truckID + "'";

                    dt = db.ExecuteDataTable();

                    DataRow dr_ASN = dt.Rows[0];

                    if (dt.DefaultView.Count > 0)
                    {
                        ASN = dr_ASN["ASN_NO"].ToString();
                    }
                    if (Status == "I")
                    {

                        if (ASN != null && ASN != "")
                        {
                            db.StrSQL = "UPDATE  WMS_RFID_TRUCK SET STATUS='I',DOCKED='" + dockdoorID + "' WHERE TRUCK_NO='" + truckID + "'";

                        }
                        else
                        {
                                if (DN != null && DN != "")
                                {
                                    db.StrSQL = "UPDATE  WMS_RFID_TRUCK SET STATUS='O',DOCKED='" + dockdoorID + "' WHERE TRUCK_NO='" + truckID + "'";

                                }

                            
                            else
                            {
                                db.StrSQL = "UPDATE  WMS_RFID_TRUCK SET STATUS=' ',DOCKED='" + dockdoorID + "' WHERE TRUCK_NO='" + truckID + "'";

                            }
                        }
                        db.ExecuteSQL();
                      //  db.Commit();

                      

                    }
                    else
                    {
                       
                        db.StrSQL = "UPDATE  WMS_RFID_TRUCK SET STATUS='"+DBNull.Value+"',DOCKED='"+DBNull.Value+"' WHERE TRUCK_NO='" + truckID + "'";

                        db.ExecuteSQL();
                       // db.Commit();

                    }
                      db.StrSQL = "SELECT STATUS FROM WMS_RFID_TRUCK WHERE TRUCK_NO='" + truckID + "'";

                        DataTable dt2 = db.ExecuteDataTable();

                        DataRow dtstatus = dt2.Rows[0];
                        
                        if (dt2.DefaultView.Count > 0)
                        {
                            O_Status = dtstatus["STATUS"].ToString();

                        }

                    var param = new DataBase.SqlParams();
                    param.Add("ContainerID", truckID);
                    param.Add("ContainerStatus", O_Status);
                    param.Add("DockDoorID", dockdoorID);
                    DateTime dtime = DateTime.Now;
                    param.Add("UpdateTime", dtime);
                    if (Status == "I")
                    {
                        param.Add("Flag", false);

                    }
                    else
                    {
                        param.Add("Flag", true);

                    }

                    db.SqlParams = param;

                    if (Status == "I")
                    {

                        db.StrSQL = " UPDATE [Innolux_DockDoor] SET ContainerID=@ContainerID, ContainerStatus=@ContainerStatus,UpdateTime=@UpdateTime,Flag=@Flag WHERE DockDoorID=@DockDoorID";

                        db.ExecuteSQL();

                    }
                    DataTable dtIP = null;
                    db.StrSQL = "SELECT IP,CaptionPanelIP,ContainerID,ContainerStatus FROM Innolux_DockDoor WHERE DockDoorID='" + dockdoorID + "'";
                    dtIP = db.ExecuteDataTable();
                    DataRow row = dtIP.Rows[0];

                    if (dtIP.DefaultView.Count > 0)
                    {
                        IP = row["IP"].ToString();
                        CaptionPanelIP = row["CaptionPanelIP"].ToString();
                        cid = row["ContainerID"].ToString();
                        cstatus = row["ContainerStatus"].ToString();

                    }

                    if (Status == "O")
                    {

                        
                        db.StrSQL = " UPDATE [Innolux_DockDoor] SET ContainerID='', ContainerStatus=@ContainerStatus,UpdateTime=@UpdateTime, Flag=@Flag WHERE ContainerID=@ContainerID";

                        db.ExecuteSQL();
                        
                    }
                    //Get IP
                 
                       

                   

                    //Save in MySql
                    if (Status == "I")
                    {
                        if (ASN != null && ASN != "")
                        {
                            // ApiDataAccess.UpdateControlDB(IP, "INSERT checkin(car_in,car_out)VALUES('I','0')");
                            ApiDataAccess.UpdateControlDB(IP, "UPDATE checkin SET  car_in='I',car_out='0'");

                            ApiDataAccess.UpdateControlDB(IP, "UPDATE status  SET  status='I'");



                        }
                        else if (DN != null && DN != "")
                        {
                            // ApiDataAccess.UpdateControlDB(IP, "INSERT checkin(car_in,car_out)VALUES('O','0')");
                            ApiDataAccess.UpdateControlDB(IP, "UPDATE checkin SET  car_in='O',car_out='0'");
                            ApiDataAccess.UpdateControlDB(IP, "UPDATE status  SET  status='O'");


                        }
                        else
                        {
                            //  ApiDataAccess.UpdateControlDB(IP, "INSERT checkin(car_in,car_out)VALUES('_','0')");

                            ApiDataAccess.UpdateControlDB(IP, "UPDATE  checkin SET car_in=' ',car_out='0'");
                            ApiDataAccess.UpdateControlDB(IP, "UPDATE status  SET  status=' '");


                        }
                        var mes = "櫃號：" + truckID + "   已進站";

                        ApiDataAccess.ControlMachine(CaptionPanelIP, mes, 0);

                    }
                    else
                    {
                       if(cstatus=="I" || cstatus =="O")
                        {
                            ApiDataAccess.GetCountProcess(dockdoorID, cid, cstatus, out strNum, out strTotal);

                            if (strNum == strTotal)
                            {


                                var msg = string.Format("櫃號：{0}({1}/{2})已離廠", truckID, strNum, strTotal);

                                ApiDataAccess.ControlMachine(CaptionPanelIP, msg, 3);

                                ApiDataAccess.UpdateControlDB(IP, "UPDATE  checkin SET car_in='0',car_out='Y'");

                            }
                            else
                            {
                                var msg = string.Format("櫃號：{0}({1}/{2})尚未完成作業", truckID, strNum, strTotal);

                                ApiDataAccess.ControlMachine(CaptionPanelIP, msg, 3);

                                ApiDataAccess.UpdateControlDB(IP, "UPDATE  checkin SET car_in='0',car_out='N'");



                                //  ApiDataAccess.UpdateControlDB(IP, "INSERT checkin(car_in,car_out)VALUES('0','N')");


                            }
                        }
                        else if(cstatus==" ")
                        {
                            var msg = string.Format("櫃號：{0})已離廠", truckID);
                            ApiDataAccess.ControlMachine(CaptionPanelIP, msg, 3);

                            ApiDataAccess.UpdateControlDB(IP, "UPDATE  checkin SET car_in='0',car_out='Y'");
                        }




                    }
                    db.Commit();

                }
                catch (Exception ex)
                {
                    var mes = ex.Message;
                    db.Rollback();
                    throw ex;
                }


            }
               
           


        }

        //public static void SaveDockDoorInfo(string truckID, string Status,string dockdoorID)
        //{

        //        string O_Status = null;
        //        DataTable dt = null;
        //        DataTable dt1 = null;
        //        string IP = null;
        //        int Total = 0;
        //        int iNum = 0;
        //        Result r = new Result();
        //        string ASN = null;
        //        string DN = null;
        //    string CaptionPanelIP = null;
        //    try
        //    {



        //        //Save in ORACLE
        //        using (var db = new DataBase.DataBase(Definition.Conn))
        //        { 

        //            db.StrSQL = "SELECT COUNT(PALLET_ID) Total, COUNT(RFID_OK_FLAG) iNum FROM WMS_RFID_CONTAINER_CHECK_V";

        //            DataTable dtable = db.ExecuteDataTable();
        //            DataRow dr = dtable.Rows[0];


        //            if (dtable.DefaultView.Count > 0)
        //            {
        //                Total = Convert.ToInt32(dr["Total"]);
        //                iNum = Convert.ToInt32(dr["iNum"]);
        //            }

        //            db.ExecuteSQL();

        //            if (Status == "I")
        //            {
        //                db.StrSQL = "SELECT ASN_NO FROM WMS_RFID_TRUCK WHERE TRUCK_NO='" + truckID + "'";

        //                dt = db.ExecuteDataTable();

        //                DataRow dr_ASN = dt.Rows[0];

        //                if (dt.DefaultView.Count > 0)
        //                {
        //                    ASN = dr_ASN["ASN_NO"].ToString();
        //                }

        //                if (ASN != null && ASN != "")
        //                {
        //                    db.StrSQL = "UPDATE  WMS_RFID_TRUCK SET STATUS='I',DOCKED='" + dockdoorID + "' WHERE TRUCK_NO='" + truckID + "'";

        //                }
        //                else
        //                {
        //                    db.StrSQL = "SELECT DN_NO FROM WMS_RFID_CONTAINER_CHECK WHERE CONTAINER_NO='" + truckID + "'";

        //                    dt1 = db.ExecuteDataTable();

        //                    if (dt1.DefaultView.Count > 0)
        //                    {
        //                        DataRow DN_dr = dt1.Rows[0];

        //                        DN = DN_dr["DN_NO"].ToString();


        //                        if (DN != null && DN != "")
        //                        {
        //                            db.StrSQL = "UPDATE  WMS_RFID_TRUCK SET STATUS='O',DOCKED='" + dockdoorID + "' WHERE TRUCK_NO='" + truckID + "'";

        //                        }

        //                    }
        //                    else
        //                    {
        //                        db.StrSQL = "UPDATE  WMS_RFID_TRUCK SET STATUS=' ',DOCKED='" + dockdoorID + "' WHERE TRUCK_NO='" + truckID + "'";

        //                    }
        //                }
        //                db.ExecuteSQL();
        //                db.Commit();

        //                db.StrSQL = "SELECT STATUS FROM WMS_RFID_TRUCK WHERE TRUCK_NO='" + truckID + "'";

        //                DataTable dt2 = db.ExecuteDataTable();

        //                DataRow row = dt2.Rows[0];

        //                if (dt2.DefaultView.Count > 0)
        //                {
        //                    O_Status = row["STATUS"].ToString();

        //                }

        //            }
        //            else
        //            {
        //                db.StrSQL = "UPDATE  WMS_RFID_TRUCK SET STATUS='" + null + "',DOCKED='" + null + "' WHERE TRUCK_NO='" + truckID + "'";

        //                db.ExecuteSQL();


        //            }

        //        }

        //        //開啟資料庫存取物件
        //        using (var db = new DataBase.DataBase(Definition.Conn))
        //        {
        //            var param = new DataBase.SqlParams();
        //            param.Add("ContainerID", truckID);
        //            param.Add("ContainerStatus", O_Status);
        //            param.Add("DockDoorID", dockdoorID);
        //            DateTime dtime = DateTime.Now;
        //            param.Add("UpdateTime", dtime);
        //            if (Status == "I")
        //            {
        //                param.Add("Flag", false);

        //            }
        //            else
        //            {
        //                param.Add("Flag", true);

        //            }

        //            db.SqlParams = param;

        //            //Get IP
        //            if (Status == "I")
        //            {
        //                DataTable dtable = null;
        //                db.StrSQL = "SELECT IP,CaptionPanelIP FROM Innolux_DockDoor WHERE DockDoorID='" + dockdoorID + "'";
        //                dtable = db.ExecuteDataTable();
        //                DataRow row = dtable.Rows[0];
        //                IP = row["IP"].ToString();
        //                CaptionPanelIP = row["CaptionPanelIP"].ToString();
        //            }
        //            else
        //            {
        //                DataTable dtable = null;
        //                db.StrSQL = "SELECT IP,CaptionPanelIP FROM Innolux_DockDoor WHERE DockDoorID='" + dockdoorID + "'";
        //                dtable = db.ExecuteDataTable();
        //                DataRow row = dtable.Rows[0];
        //                IP = row["IP"].ToString();
        //                CaptionPanelIP = row["CaptionPanelIP"].ToString();
        //            }

        //            if (Status == "I")
        //            {

        //                db.StrSQL = " UPDATE [Innolux_DockDoor] SET ContainerID=@ContainerID, ContainerStatus=@ContainerStatus,UpdateTime=@UpdateTime,Flag=@Flag WHERE DockDoorID=@DockDoorID";

        //                db.ExecuteSQL();

        //                var mes = "櫃號：" + truckID + "   已進站";

        //                ApiDataAccess.ControlMachine(CaptionPanelIP, mes, 0);

        //            }
        //            else
        //            {
        //                db.StrSQL = " UPDATE [Innolux_DockDoor] SET ContainerID='', ContainerStatus='',UpdateTime=@UpdateTime, Flag=@Flag WHERE ContainerID=@ContainerID";

        //                db.ExecuteSQL();
        //            }
        //            //Save in MySql
        //            if (Status == "I")
        //            {
        //                if (ASN != null && ASN != "")
        //                {
        //                    // ApiDataAccess.UpdateControlDB(IP, "INSERT checkin(car_in,car_out)VALUES('I','0')");
        //                    ApiDataAccess.UpdateControlDB(IP, "UPDATE checkin SET  car_in='I',car_out='0'");

        //                    ApiDataAccess.UpdateControlDB(IP, "UPDATE status  SET  status='I'");



        //                }
        //                else if (DN != null && DN != "")
        //                {
        //                    // ApiDataAccess.UpdateControlDB(IP, "INSERT checkin(car_in,car_out)VALUES('O','0')");
        //                    ApiDataAccess.UpdateControlDB(IP, "UPDATE checkin SET  car_in='O',car_out='0'");
        //                    ApiDataAccess.UpdateControlDB(IP, "UPDATE status  SET  status='O'");


        //                }
        //                else
        //                {
        //                    //  ApiDataAccess.UpdateControlDB(IP, "INSERT checkin(car_in,car_out)VALUES('_','0')");

        //                    ApiDataAccess.UpdateControlDB(IP, "UPDATE  checkin SET car_in=' ',car_out='0'");
        //                    ApiDataAccess.UpdateControlDB(IP, "UPDATE status  SET  status=' '");


        //                }


        //            }
        //            else
        //            {
        //                if (Total == iNum)
        //                {


        //                    var msg = string.Format("櫃號：{0}({1}/{2})已離廠", truckID, iNum, Total);

        //                    ApiDataAccess.ControlMachine(CaptionPanelIP, msg, 3);

        //                    ApiDataAccess.UpdateControlDB(IP, "UPDATE  checkin SET car_in='0',car_out='Y'");

        //                }
        //                else
        //                {
        //                    var msg = string.Format("櫃號：{0}({1}/{2})尚未完成作業", truckID, iNum, Total);

        //                    ApiDataAccess.ControlMachine(CaptionPanelIP, msg, 3);

        //                    ApiDataAccess.UpdateControlDB(IP, "UPDATE  checkin SET car_in='0',car_out='N'");



        //                    //  ApiDataAccess.UpdateControlDB(IP, "INSERT checkin(car_in,car_out)VALUES('0','N')");

        //                }
        //            }




        //        }
        //    }
        //    catch
        //    {

        //    }


        //}

        public static DataTable GetDockDoorListDetails()
        {
            //開啟資料庫存取物件
            using (var db = new DataBase.DataBase(Definition.Conn))
            {
                //判斷Status成功或失敗
                DataTable dt = null;
                db.StrSQL = "SELECT ContainerID,Flag FROM Innolux_DockDoor WHERE 1=1";
                dt = db.ExecuteDataTable();
                return dt;
            }
        }

    }
}
