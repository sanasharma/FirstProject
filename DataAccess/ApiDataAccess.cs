using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Web;
using Models.Base;
using Models.ApiModel;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Tool;
using System.Threading.Tasks;

namespace DataAccess
{
    public class ApiDataAccess
    {
        static string DB_Prestr = "NKWMS_DB.";
        //static string DB_Prestr = "";
        /// <summary>貨櫃進站函式</summary>
        public static PostResult AddContainer(Container m)
        {
            PostResult r = new PostResult();
            string strContainerID = "", strContainerStatus = "";

            try
            {
                //判斷是否有貨櫃標籤
                if (m.containers.Count == 0) { throw new Exception("無貨櫃標籤"); }
                else { m.containers = m.containers.Select(x => x.Trim()).ToList(); } //貨櫃號碼去空白

                using (var db = new DataBase.DataBase(Definition.Conn))
                {
                    //檢查是否誤讀到其它貨櫃
                    for (int i = m.containers.Count - 1; i >= 0; i--)
                    {
                        string CID = m.containers[i].Substring(1);
                        db.StrSQL = "SELECT COUNT(*) FROM Innolux_DockDoor WHERE IP <> '" + m.ip + "' AND ContainerID = '" + CID + "'";
                        //ExecuteScalar 返回單行單列的結果集
                        if (db.ExecuteScalar() != "0") m.containers.RemoveAt(i);
                    }

                    //判斷過濾後是否只剩一筆貨櫃ID
                    if (m.containers.Count == 1)
                    {
                        strContainerID = m.containers[0].Substring(1);
                        strContainerStatus = GetContainerStatus(strContainerID);

                        //更新控制器貨櫃狀態
                        UpdateControlDB(m.ip, "UPDATE status SET status='" + strContainerStatus + "'");

                        r.result = strContainerStatus;
                        r.msg = "櫃號：" + strContainerID + "已進站";
                    }
                    else
                    {
                        throw new Exception("無貨櫃標籤");
                    }
                }
            }
            catch (Exception ex)
            {
                r.result = "N";
                r.msg = ex.Message;
            }
            finally
            {
                using (var db = new DataBase.DataBase(Definition.Conn))
                {
                    //記錄碼頭目前停靠的貨櫃ID與狀態
                    string strAlarm = (r.result == "N" ? "Y" : "");
                    db.StrSQL = "UPDATE Innolux_DockDoor SET ContainerID = '" + strContainerID + "', ContainerStatus = '" + r.result + "', Alarm = '" + strAlarm + "' WHERE IP = '" + m.ip + "'";
                    db.ExecuteSQL();

                }

                //控制語音與字幕機
                DataTable dtDockDoor = DockDoorDataAccess.GetDockDoorList(null, m.ip, null, null, new Pages());
                DataRow rowDoor = dtDockDoor.Rows[0];
                string strCaptionPanelIP = rowDoor["CaptionPanelIP"].ToString();
                ControlMachine(strCaptionPanelIP, r.msg, 0);
            }

            return r;
        }

        /// <summary>貨櫃離站函式</summary>
        public static PostResult DelContainer(Container m)
        {
            PostResult r = new PostResult();

            //取得此碼頭資料
            DataTable dtDockDoor = DockDoorDataAccess.GetDockDoorList(null, m.ip, null, null, new Pages());
            DataRow rowDoor = dtDockDoor.Rows[0];
            string strDockDoorID = rowDoor["DockDoorID"].ToString();
            string strContainerID = rowDoor["ContainerID"].ToString();
            string strContainerStatus = rowDoor["ContainerStatus"].ToString();
            string strCaptionPanelIP = rowDoor["CaptionPanelIP"].ToString();

            try
            {
                string[] StatusList = new string[] { "I", "O" };
                if (StatusList.Contains(strContainerStatus))
                {
                    //判斷裝卸載數量
                    string strNum, strTotal;
                    GetCountProcess(strDockDoorID, strContainerID, strContainerStatus, out strNum, out strTotal);
                    if (strNum == strTotal)
                    {
                        r.result = "Y";
                        r.msg = string.Format("櫃號：{0}({1}/{2})已離廠", strContainerID, strNum, strTotal);
                    }
                    else
                    {
                        throw new Exception(string.Format("櫃號：{0}({1}/{2})尚未完成作業", strContainerID, strNum, strTotal));
                    }
                }
                else
                {
                    r.result = "Y";
                    r.msg = string.Format("櫃號：{0}已離廠", strContainerID);
                }
            }
            catch (Exception ex)
            {
                r.result = "N";
                r.msg = ex.Message;
            }
            finally
            {
                //清空碼頭目前停靠的貨櫃ID
                using (var db = new DataBase.DataBase(Definition.Conn))
                {
                    db.StrSQL = "UPDATE Innolux_DockDoor SET ContainerID = '', ContainerStatus = '', Alarm = '' WHERE IP = '" + m.ip + "'";
                    db.ExecuteSQL();
                }
            }

            ControlMachine(strCaptionPanelIP, r.msg, 3);
            return r;
        }

        /// <summary>上報至客戶端系統</summary>
        public static PostResult Post(PostModel m)
        {
            PostResult r = new PostResult();
            string strToDay = DateTime.Now.ToString("yyyy/MM/dd");
            DataTable dtCheckList = new DataTable();

            //取得此碼頭資料
            DataTable dtDockDoor = DockDoorDataAccess.GetDockDoorList(null, m.ip, null, null, new Pages());
            DataRow rowDoor = dtDockDoor.Rows[0];
            string strDockDoorID = rowDoor["DockDoorID"].ToString();
            string strContainerID = rowDoor["ContainerID"].ToString();
            string strContainerStatus = rowDoor["ContainerStatus"].ToString();
            string strCaptionPanelIP = rowDoor["CaptionPanelIP"].ToString();
            string strFilterRules = rowDoor["FilterRules"].ToString();
            int iMask = int.Parse(rowDoor["Mask"].ToString());
            int iMaskOut = int.Parse(rowDoor["MaskOut"].ToString());

            //去除棧板的前後空白
            foreach (var item in m.pallets) item.pallet = item.pallet.Trim();

            //記錄原始上報資料
            string strData = JsonConvert.SerializeObject(m);

            //棧板白名單過濾機制
            if (strFilterRules.Trim() != "")
            {
                string[] rules = strFilterRules.Trim().Split(',');
                m.pallets.RemoveAll(x => !rules.Contains(x.pallet.Substring(0, 2)));
            }

            //最得訊號最大值當作浮動參考值
            float maxRefer = 0;
            foreach (var item in m.pallets)
            {
                if (maxRefer < (int.Parse(item.count) * float.Parse(item.rssi)))
                {
                    maxRefer = (int.Parse(item.count) * float.Parse(item.rssi));
                }
            }

            //移除低於Mask過濾值的標籤
            if (strContainerStatus == "I")
            {
                m.pallets.RemoveAll(x => (int.Parse(x.count) * float.Parse(x.rssi)) < (maxRefer * iMask / 100));
            }
            else if (strContainerStatus == "O")
            {
                m.pallets.RemoveAll(x => (int.Parse(x.count) * float.Parse(x.rssi)) < (maxRefer * iMaskOut / 100));
            }

            float kk = (maxRefer * iMask / 100);

            try
            {
                //判斷有無棧板資訊
                if (m.pallets.Count == 0) throw new Exception("無載貨資訊");

                using (var db = new DataBase.OracleDataBase(Definition.WMSConn))
                {
                    //過濾當天已上報的棧板
                    string strRFIDList = string.Join(",", m.pallets.Select(x => "'" + x.pallet + "'"));
                    db.StrSQL = "SELECT RFID FROM " + DB_Prestr + "WMS_RFID_OK_LIST WHERE RFID IN (" + strRFIDList + ") AND TO_CHAR(CREATION_DATE,'yyyy/MM/dd') = '" + strToDay + "'";
                    var list = db.ExecuteDataTable().AsEnumerable().Select(x => x.Field<string>("RFID"));
                    m.pallets.RemoveAll(x => list.Contains(x.pallet));

                    if (m.pallets.Count > 0)
                    {
                        //判斷貨櫃進料或出料狀態(I:收料,O:出料)
                        if (strContainerStatus == "I")
                        {
                            r = POST_IN(strDockDoorID, strContainerID, strContainerStatus, m);
                        }
                        else if (strContainerStatus == "O")
                        {
                            r = POST_OUT(strDockDoorID, strContainerID, strContainerStatus, m);
                        }
                        else
                        {
                            throw new Exception("無" + strContainerID + "貨櫃資訊");
                        }
                    }
                    else
                    {
                        throw new Exception("無載貨資訊");
                    }
                }
            }
            catch (Exception ex)
            {
                //若無讀到棧板資訊則result=0
                r.result = (ex.Message == "無載貨資訊" ? "0" : "N");
                r.msg = ex.Message;

                if (r.result == "N")
                {
                    //逐筆新增非法資料
                    using (var db = new DataBase.OracleDataBase(Definition.WMSConn))
                    {
                        foreach (var item in m.pallets)
                        {

                            db.StrSQL = @"INSERT INTO " + DB_Prestr + "WMS_RFID_NG_LIST (DOCK_DOOR_NO, RFID, ERR_MSG) VALUES('" + strDockDoorID + "', '" + item.pallet + "', '" + ex.Message + "')";
                            db.ExecuteSQL();
                        }
                    }
                }
            }
            finally
            {
                if (strContainerStatus == "I")
                {
                    AddTagLog(strDockDoorID, iMask, m, r, strData);
                }
                else if (strContainerStatus == "O")
                {
                    AddTagLog(strDockDoorID, iMaskOut, m, r, strData);
                }

                //若有正常回應，則控制字幕機
                string[] strs = new string[] { "Y", "A", "N" };
                if (strs.Contains(r.result)) { ControlMachine(strCaptionPanelIP, r.msg, 0); }
            }

            return r;
        }

        /// <summary>清除警報</summary>
        public static void ClearAlarm(string id)
        {
            //取得指定警報中的碼頭
            DataTable dtDoor = DockDoorDataAccess.GetDockDoorList(id, null, null, null, new Pages());

            if (dtDoor.Rows.Count > 0)
            {
                DataRow row = dtDoor.Rows[0];
                string strIP = row["IP"].ToString();
                string strContainerID = row["ContainerID"].ToString();
                string strCaptionPanelIP = row["CaptionPanelIP"].ToString();

                //修改Reader DB，以觸發DB Trigger
                UpdateControlDB(strIP, "UPDATE alarm SET alarm='" + DateTime.Now.ToString("yyyyMMddHHmmss") + "'");

                //清除碼頭的異常狀態
                using (var db = new DataBase.DataBase(Definition.Conn))
                {
                    db.StrSQL = "UPDATE Innolux_DockDoor SET Alarm = '' WHERE DockDoorID = '" + id + "'";
                    db.ExecuteSQL();
                }

                //若有貨櫃停靠，則播放最後一次成功訊息
                if (strContainerID != "")
                {
                    using (var db = new DataBase.DataBase(Definition.Conn))
                    {
                        string strToday = DateTime.Now.ToString("yyyyMMdd");
                        db.StrSQL = "SELECT TOP 1 Msg FROM Innolux_TagLog WHERE (DockDoorID = '" + id + "') AND (Status IN ('Y','A')) AND (Msg LIKE '%" + strContainerID + "%') AND (Convert(varchar, DateTime, 112) = '" + strToday + "') ORDER BY DateTime DESC";
                        string strMsg = db.ExecuteScalar() ?? "";
                        ControlMachine(strCaptionPanelIP, strMsg, 0);
                    }
                }
                else
                {
                    ControlMachine(strCaptionPanelIP, "", 1);
                }
            }
        }

        /// <summary>新增讀取Tag Log</summary>
        private static void AddTagLog(string _DockDoorID, int _Mask, PostModel m, PostResult r, string _Data)
        {
            using (var db = new DataBase.DataBase(Definition.Conn))
            {
                var param = new DataBase.SqlParams();
                param.Add("DockDoorID", _DockDoorID);
                param.Add("Mask", _Mask);
                param.Add("IP", m.ip);
                param.Add("DateTime", DateTime.Now);
                param.Add("Status", r.result);
                param.Add("Alarm", r.result == "N" ? "Y" : "");
                param.Add("Msg", r.msg);
                param.Add("Data", _Data);
                param.Add("PostData", JsonConvert.SerializeObject(m));
                db.SqlParams = param;

                db.StrSQL = "UPDATE Innolux_DockDoor SET Alarm = @Alarm WHERE IP = @IP;" +
                            "INSERT [Innolux_TagLog](DockDoorID, IP, DateTime, Mask, Status, Msg, Data, PostData) VALUES(@DockDoorID, @IP, @DateTime, @Mask, @Status, @Msg, @Data, @PostData);";
                db.ExecuteSQL();
            }
        }

        /// <summary>出料邏輯</summary>
        private static PostResult POST_OUT(string DockDoorID, string ContainerID, string ContainerStatus, PostModel m)
        {
            PostResult r = new Models.ApiModel.PostResult();
            using (var db = new DataBase.OracleDataBase(Definition.WMSConn))
            {
                //判斷前次與此次上報時間，大於3秒則清空貨櫃清單，重新計算
                /*
                if (((DateTime.Now.Ticks - Definition.LastPostTime.Ticks) / 10000000) > 3) Definition.CONTAINERS.Clear();
                Definition.CONTAINERS.Add(ContainerID);
                Definition.LastPostTime = DateTime.Now;

                Thread.Sleep(3500); //等待時間內是否還有資料繼續上報
                */

                //讀取指定棧板號碼的資料
                string strRFIDList = string.Join(",", m.pallets.Select(x => "'" + x.pallet + "'"));
                db.StrSQL = "SELECT * FROM " + DB_Prestr + "WMS_RFID_CHECK_LIST_VIEW WHERE (DOCK_DOOR_NO = '" + DockDoorID + "') AND (RFID IN (" + strRFIDList + "))";
                DataTable dtCheckList = db.ExecuteDataTable();

                #region 檢查RFID資訊是否異常
                foreach (PalletItem item in m.pallets)
                {
                    DataRow[] rows = dtCheckList.Select("RFID = '" + item.pallet + "'");
                    if (rows.Length == 1)
                    {
                        string strStatus = rows[0]["STATUS"].ToString();
                        if (strStatus != "0") throw new Exception(item.pallet + "異常資訊");
                    }
                    else
                    {
                        throw new Exception(item.pallet + "異常資訊");
                    }
                }
                #endregion

                //判斷是否裝錯櫃
                //string strContainers = string.Join(",", Definition.CONTAINERS.Select(x => "'" + x + "'"));
                db.StrSQL = "SELECT CONTAINER_NO, PALLET_ID, RFID_OK_FLAG FROM " + DB_Prestr + "WMS_RFID_CONTAINER_CHECK_V WHERE (CONTAINER_NO = '" + ContainerID + "') AND (PALLET_ID IN (" + strRFIDList + "))";
                DataTable dtPallet = db.ExecuteDataTable();
                var listPallet = dtPallet.AsEnumerable().Select(x => x.Field<string>("PALLET_ID"));
                var listErrorPallet = m.pallets.FindAll(x => !listPallet.Contains(x.pallet)).Select(x => x.pallet);
                if (listErrorPallet.Count() > 0)
                {
                    throw new Exception(string.Join(",", listErrorPallet) + " 不屬於該貨櫃");
                }

                //過濾已用其他方式過帳的棧板資料
                listPallet = dtPallet.AsEnumerable().Where(x => x.Field<string>("RFID_OK_FLAG") == null && x.Field<string>("CONTAINER_NO") == ContainerID).Select(x => x.Field<string>("PALLET_ID"));
                var Rows = dtCheckList.AsEnumerable().Where(x => listPallet.Contains(x.Field<string>("RFID")));
                m.pallets = m.pallets.FindAll(x => listPallet.Contains(x.pallet));

                if(Rows.Count() > 0)
                {
                    //逐筆執行過帳
                    foreach (DataRow row in Rows)
                    {
                        //設定DB參數值
                        var param = new DataBase.SqlParams();
                        foreach (DataColumn col in dtCheckList.Columns)
                            param.Add(col.ColumnName, row[col.ColumnName].ToString());

                        db.SqlParams = param;
                        db.StrSQL = "INSERT INTO " + DB_Prestr + "WMS_RFID_OK_LIST(PLANT, DOCK_DOOR_NO, ACTION, RFID, RESERVED_01, RESERVED_02, RESERVED_03, RESERVED_04, RESERVED_05, RESERVED_06, RESERVED_07, RESERVED_08, RESERVED_09, RESERVED_10, RESERVED_11, RESERVED_12, RESERVED_13, RESERVED_14, RESERVED_15, RESERVED_16, RESERVED_17, RESERVED_18, RESERVED_19, RESERVED_20) " +
                                    "VALUES(:PLANT, :DOCK_DOOR_NO, :ACTION, :RFID, :RESERVED_01, :RESERVED_02, '" + ContainerID + "', :RESERVED_04, :RESERVED_05, :RESERVED_06, :RESERVED_07, :RESERVED_08, :RESERVED_09, :RESERVED_10, :RESERVED_11, :RESERVED_12, :RESERVED_13, :RESERVED_14, :RESERVED_15, :RESERVED_16, :RESERVED_17, :RESERVED_18, :RESERVED_19, :RESERVED_20)";
                        db.ExecuteSQL();
                    }

                    //判斷裝載數量
                    string strNum, strTotal;
                    GetCountProcess(DockDoorID, ContainerID, ContainerStatus, out strNum, out strTotal);
                    if (strNum != strTotal)
                    {
                        r.result = "Y";
                        r.msg = string.Format("櫃號:{0}({1}/{2})裝櫃中", ContainerID, strNum, strTotal);
                    }
                    else
                    {
                        r.result = "A";
                        r.msg = string.Format("櫃號:{0}({1}/{2})裝櫃完成", ContainerID, strNum, strTotal);
                    }
                }
                else
                {
                    throw new Exception("無載貨資訊");
                }
            }

            return r;
        }

        /// <summary>收料邏輯</summary>
        private static PostResult POST_IN(string DockDoorID, string ContainerID, string ContainerStatus, PostModel m)
        {
            PostResult r = new Models.ApiModel.PostResult();
            using (var db = new DataBase.OracleDataBase(Definition.WMSConn))
            {
                //讀取指定棧板號碼的資料
                //20180214 增加 RESERVED_03 = ContainerID 條件 --> 要讓一單多車棧板資料可以上報(查詢出的資料只有一筆才合法)
                string strRFIDList = string.Join(",", m.pallets.Select(x => "'" + x.pallet + "'"));
                string strSQL = "SELECT * FROM " + DB_Prestr + "WMS_RFID_CHECK_LIST_VIEW WHERE (RESERVED_03 = '" + ContainerID + "') AND (DOCK_DOOR_NO = '" + DockDoorID + "') AND (RFID IN (" + strRFIDList + "))";
                db.StrSQL = strSQL;
                DataTable dtCheckList = db.ExecuteDataTable();

                using (var dba = new DataBase.DataBase(Definition.Conn))
                {
                    //20180214 增加 --> 記錄 Select View SQL 語法
                    dba.StrSQL = "INSERT DebugSQL (sql,updatetime) VALUES ('"+ strSQL.Replace("'","") +"','"+ DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") +"')";
                    dba.ExecuteSQL();
                }

                #region 檢查RFID資訊是否異常
                foreach (PalletItem item in m.pallets)
                {
                    DataRow[] rows = dtCheckList.Select("RFID = '" + item.pallet + "'");
                    if (rows.Length == 1)
                    {
                        string strStatus = rows[0]["STATUS"].ToString();
                        if (strStatus != "0") throw new Exception(item.pallet + "異常資訊");
                    }
                    else
                    {
                        throw new Exception(item.pallet + "異常資訊");
                    }
                }
                #endregion

                //判斷卸櫃資料是否正確
                //20180226 增加 RESERVED_05 == Definition.AppPlant 條件 --> 確認屬於該廠的進貨
                var listPallet = dtCheckList.AsEnumerable().Where(x => x.Field<string>("RESERVED_03") == ContainerID && x.Field<string>("RESERVED_05") == Definition.AppPlant).Select(x => x.Field<string>("RFID")).ToList();
                var listErrorPallet = m.pallets.FindAll(x => !listPallet.Contains(x.pallet)).Select(x => x.pallet);
                if (listErrorPallet.Count() > 0)
                {
                    throw new Exception(string.Join(",", listErrorPallet) + " 不屬於該廠區");
                }

                //逐筆執行過帳
                foreach (DataRow row in dtCheckList.Rows)
                {
                    //設定DB參數值
                    var param = new DataBase.SqlParams();
                    foreach (DataColumn col in dtCheckList.Columns)
                        param.Add(col.ColumnName, row[col.ColumnName].ToString());

                    db.SqlParams = param;
                    db.StrSQL = "INSERT INTO " + DB_Prestr + "WMS_RFID_OK_LIST(PLANT, DOCK_DOOR_NO, ACTION, RFID, RESERVED_01, RESERVED_02, RESERVED_03, RESERVED_04, RESERVED_05, RESERVED_06, RESERVED_07, RESERVED_08, RESERVED_09, RESERVED_10, RESERVED_11, RESERVED_12, RESERVED_13, RESERVED_14, RESERVED_15, RESERVED_16, RESERVED_17, RESERVED_18, RESERVED_19, RESERVED_20) " +
                                "VALUES(:PLANT, :DOCK_DOOR_NO, :ACTION, :RFID, :RESERVED_01, :RESERVED_02, :RESERVED_03, :RESERVED_04, :RESERVED_05, :RESERVED_06, :RESERVED_07, :RESERVED_08, :RESERVED_09, :RESERVED_10, :RESERVED_11, :RESERVED_12, :RESERVED_13, :RESERVED_14, :RESERVED_15, :RESERVED_16, :RESERVED_17, :RESERVED_18, :RESERVED_19, :RESERVED_20)";
                    db.ExecuteSQL();
                }

                //判斷卸載數量
                string strNum, strTotal;
                GetCountProcess(DockDoorID, ContainerID, ContainerStatus, out strNum, out strTotal);
                if (strNum != strTotal)
                {
                    r.result = "Y";
                    r.msg = string.Format("櫃號:{0}({1}/{2})收料中", ContainerID, strNum, strTotal);
                }
                else
                {
                    r.result = "A";
                    r.msg = string.Format("櫃號:{0}({1}/{2})收料完成", ContainerID, strNum, strTotal);
                }
            }

            return r;
        }

        /// <summary>控制字幕機</summary>
        public static void ControlMachine(string CaptionPanelIP, string Message, int Loop)
        {
            //異步控制字幕機
            var task1 = Task.Factory.StartNew(() =>
            {
                //判斷此次與前次的寫入時間是否大於200毫秒，以避免短時間內字幕機控制出錯
                if (((DateTime.Now.Ticks - Definition.LastPanelUpdateTime.Ticks) / 10000) < 200) { Thread.Sleep(200); }
                Definition.LastPanelUpdateTime = DateTime.Now;

                //內容，循環次數
                object[] objs = new object[] { Message, Loop };

                //顯示字幕機
                CaptionPanel pal = new CaptionPanel(CaptionPanelIP);
                pal.SendText(objs);
            });
        }

        /// <summary>計算裝載量</summary>
        public static void GetCountProcess(string DockDoorID, string ContainerID, string Status, out string strNum, out string strTotal)
        {
            DataTable dtTotal = null;
            if (Status == "I")
            {
                using (var db = new DataBase.OracleDataBase(Definition.WMSConn))
                {
                    //取得此貨櫃的進料筆數
                    //20180226 增加 RESERVED_05 == Definition.AppPlant 條件 --> 分別計算各廠的棧板數
                    //20180309 增加 RESERVED_20 IS NULL 條件 --> 己離廠的貨櫃不計算 Count 數
                    db.StrSQL = "SELECT COUNT(v.RFID) Total, COUNT(l.RFID) iNum " +
                                "FROM " + DB_Prestr + "WMS_RFID_CHECK_LIST_VIEW v " +
                                "LEFT OUTER JOIN " + DB_Prestr + "WMS_RFID_OK_LIST l ON (l.RESERVED_01 = v.RESERVED_01) AND (l.RFID = v.RFID) " +
                                "WHERE (v.DOCK_DOOR_NO = '" + DockDoorID + "') AND (v.RESERVED_03 = '" + ContainerID + "') AND (v.RESERVED_05 = '" + Definition.AppPlant + "')" +
                                " AND (v.RESERVED_20 IS NULL) ";
                    dtTotal = db.ExecuteDataTable();
                }
            }
            else if (Status == "O")
            {
                using (var db = new DataBase.OracleDataBase(Definition.WMSConn))
                {
                    //取得此貨櫃的出貨筆數
                    db.StrSQL = "SELECT COUNT(PALLET_ID) Total, COUNT(RFID_OK_FLAG) iNum FROM " + DB_Prestr + "WMS_RFID_CONTAINER_CHECK_V WHERE CONTAINER_NO = '" + ContainerID + "'";
                    dtTotal = db.ExecuteDataTable();
                }
            }

            strNum = dtTotal.Rows[0]["iNum"].ToString();
            strTotal = dtTotal.Rows[0]["Total"].ToString();
        }

        //Special Truck Details


        //public static DataTable GetSpecial_TruckDetails()
        //{
        //    DataTable dt = null;

        //    string Io_no = System.Web.Configuration.WebConfigurationManager.AppSettings["IO_No"];

        //    using (var db = new DataBase.OracleDataBase(Definition.WMSConn))
        //    {
        //        var param = new DataBase.SqlParams();

        //      //  param.Add("TRUCK_NO", TRUCK_NO);

        //        param.Add("IO_NO", Io_no);

        //        db.SqlParams = param;

        //        string strWHERE = "";

        //     //   if (!string.IsNullOrEmpty(TRUCK_NO)) strWHERE += " AND TRUCK_NO = '" + TRUCK_NO + "' ";


        //        if (!string.IsNullOrEmpty(Io_no)) strWHERE += " AND IO_NO = '" + Io_no + "' ";

        //        db.StrSQL = "SELECT TRUCK_NO FROM WMS_RFID_TRUCK WHERE 1=1 AND DOCKED IS NULL " + strWHERE;

        //        dt = db.ExecuteDataTable();

        //        return dt;
        //    }
        //}

        public static DataTable GetSpecial_TruckDetails()
        {
            DataTable dt = null;

            string Io_no = System.Web.Configuration.WebConfigurationManager.AppSettings["IO_No"];

            using (var db = new DataBase.DataBase(Definition.Conn))
            {
                var param = new DataBase.SqlParams();

                //  param.Add("TRUCK_NO", TRUCK_NO);

                param.Add("IO_NO", Io_no);

                db.SqlParams = param;

                string strWHERE = "";

                //   if (!string.IsNullOrEmpty(TRUCK_NO)) strWHERE += " AND TRUCK_NO = '" + TRUCK_NO + "' ";


                if (!string.IsNullOrEmpty(Io_no)) strWHERE += " AND IO_NO = '" + Io_no + "' ";

                db.StrSQL = "SELECT DISTINCT TRUCK_NO FROM WMS_RFID_TRUCK WHERE 1=1 AND (DOCKED IS NULL OR DOCKED='') " + strWHERE;

                dt = db.ExecuteDataTable();

                return dt;
            }
        }
        public static DataTable GetSpecialTruckFilteredDetails(string TRUCK_NO)
        {
            DataTable dt = null;

            string Io_no = System.Web.Configuration.WebConfigurationManager.AppSettings["IO_No"];

            using (var db = new DataBase.DataBase(Definition.Conn))
            {
                var param = new DataBase.SqlParams();

                param.Add("TRUCK_NO", TRUCK_NO);

                param.Add("IO_NO", Io_no);

                db.SqlParams = param;

                string strWHERE = "";

                if (!string.IsNullOrEmpty(Io_no)) strWHERE += " AND IO_NO = '" + Io_no + "' ";

                db.StrSQL = "SELECT DISTINCT TRUCK_NO FROM WMS_RFID_TRUCK WHERE 1=1 AND  (DOCKED IS NULL OR DOCKED='')  AND TRUCK_NO = '" + TRUCK_NO +"'" + strWHERE;

                dt = db.ExecuteDataTable();

                return dt;
            }
        }

        //public static DataTable GetSpecialTruckFilteredDetails(string TRUCK_NO)
        //{
        //    DataTable dt = null;

        //    string Io_no = System.Web.Configuration.WebConfigurationManager.AppSettings["IO_No"];

        //    using (var db = new DataBase.OracleDataBase(Definition.WMSConn))
        //    {
        //        var param = new DataBase.SqlParams();

        //        param.Add("TRUCK_NO", TRUCK_NO);

        //        param.Add("IO_NO", Io_no);

        //        db.SqlParams = param;

        //        string strWHERE = "";

        //        if (!string.IsNullOrEmpty(Io_no)) strWHERE += " AND IO_NO = '" + Io_no + "' ";

        //        db.StrSQL = "SELECT TRUCK_NO FROM WMS_RFID_TRUCK WHERE 1=1 AND DOCKED IS NULL AND TRUCK_NO LIKE '" + TRUCK_NO + "%'" + strWHERE;

        //        dt = db.ExecuteDataTable();

        //        return dt;
        //    }
        //}

        /// <summary>取得貨櫃的作業狀態</summary>
        public static string GetContainerStatus(string ContainerID)
        {
            string strContainerStatus;
            using (var db1 = new DataBase.OracleDataBase(Definition.WMSConn))
            {
                //檢查哨口發卡記錄
                db1.StrSQL = "SELECT COUNT(*) FROM " + DB_Prestr + "WMS_RFID_TRUCK WHERE (TRUCK_NO = '" + ContainerID + "') AND (ASN_NO IS NOT NULL)";
                int iResult = int.Parse(db1.ExecuteScalar());

                if (iResult > 0)
                {
                    //若有ASN號碼，則為進料I
                    strContainerStatus = "I";
                }
                else
                {
                    //若無ASN號碼或無記錄，進而檢查是否為出料狀態
                    db1.StrSQL = "SELECT COUNT(*) FROM " + DB_Prestr + "WMS_RFID_CONTAINER_CHECK_V WHERE CONTAINER_NO = '" + ContainerID + "'";
                    iResult = int.Parse(db1.ExecuteScalar());

                    strContainerStatus = (iResult > 0 ? "O" : " ");
                }
            }

            return strContainerStatus;
        }

        /// <summary>更新控制器DB</summary>
        public static void UpdateControlDB(string IP, string SQL)
        {
            //更新控制器的貨櫃狀態
            string strControlConn = Definition.ControlConn.Replace("{IP}", IP);
            using (MySqlConnection conn = new MySqlConnection(strControlConn))
            {
                using (MySqlCommand cmd = new MySqlCommand(SQL, conn))
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}