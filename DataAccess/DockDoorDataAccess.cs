using System;
using System.Linq;
using System.Data;
using System.Web;
using Models.Base;
using Models.ViewModel.DockDoor;
using MySql.Data.MySqlClient;
using Tool;

namespace DataAccess
{
    public class DockDoorDataAccess
    {
        /// <summary>取得DockDoor資料</summary>
        public static DataTable GetDockDoorList(string DockDoorID, string IP, string Alarm, string ContainerID, Pages _page)
        {
            //開啟資料庫存取物件
            using (var db = new DataBase.DataBase(Definition.Conn))
            {
                //資料庫參數
                var param = new DataBase.SqlParams();
                param.Add("DockDoorID", DockDoorID);
                param.Add("IP", IP);
                param.Add("Alarm", Alarm);
                param.Add("ContainerID", ContainerID);
                db.SqlParams = param;

                //條件式
                string strWHERE = "";
                if (!string.IsNullOrEmpty(DockDoorID)) strWHERE += " AND DockDoorID = @DockDoorID ";
                if (!string.IsNullOrEmpty(IP)) strWHERE += " AND IP = @IP ";
                if (!string.IsNullOrEmpty(Alarm)) strWHERE += " AND Alarm = @Alarm ";
                if (!string.IsNullOrEmpty(ContainerID)) strWHERE += " AND ContainerID = @ContainerID ";

                //取回資料
                int iTotal;
                var sql = @"SELECT d.*, f.FilterName, f.FilterRules FROM Innolux_DockDoor d
                            INNER JOIN Innolux_Filter f ON f.FilterCode = d.FilterCode 
                            WHERE 1=1" + strWHERE;
                var dt = db.ExecuteDataTable(sql, "DockDoorID", _page.PageIndex, _page.PageSize, out iTotal);
                _page.TotalRows = iTotal;

                return dt;
            }
        }

        /// <summary>儲存DockDoor資料</summary>
        public static void SaveDockDoorInfo(DockDoorDetailViewModel m)
        {
            //開啟資料庫存取物件
            using (var db = new DataBase.DataBase(Definition.Conn))
            {
                //資料庫參數
                var param = new DataBase.SqlParams();
                param.Add("ID", m.ID);
                param.Add("DockDoorID", m.DockDoorID);
                param.Add("Remark", m.Remark);
                param.Add("Locate", m.Locate);
                param.Add("IP", m.IP);
                param.Add("CaptionPanelIP", m.CaptionPanelIP);
                param.Add("FilterCode", m.FilterCode);
                param.Add("Mask", m.Mask);
                param.Add("MaskOut", m.MaskOut);
                param.Add("Alarm", "N");
                param.Add("ContainerID", "");
                param.Add("ContainerStatus", "");
                param.Add("Account", Definition.UserInfo.Account);
                db.SqlParams = param;

                if (m.ID == 0)
                {
                    //新增
                    db.StrSQL =
                        @"DECLARE @Count int;
                          SET @Count = (SELECT COUNT(*) FROM [Innolux_DockDoor] WHERE (DockDoorID = @DockDoorID) OR (IP = @IP));
                          IF (@Count = 0) BEGIN
                             INSERT [Innolux_DockDoor](DockDoorID, Remark, Locate, IP, CaptionPanelIP, FilterCode, Mask, MaskOut, Alarm, ContainerID, ContainerStatus, CreateTime, Creater, UpdateTime, Updater) VALUES(@DockDoorID, @Remark, @Locate, @IP, @CaptionPanelIP, @FilterCode, @Mask, @MaskOut, @Alarm, @ContainerID, @ContainerStatus, GetDate(), @Account, GetDate(), @Account);
                          END ELSE BEGIN
                             THROW 51000, '碼頭編號或IP已存在', 1;
                          END";
                }
                else
                {
                    //修改
                    db.StrSQL =
                        @"DECLARE @Count int;
                          SET @Count = (SELECT COUNT(*) FROM [Innolux_DockDoor] WHERE (ID <> @ID) AND (DockDoorID = @DockDoorID OR IP = @IP));
                          IF (@Count = 0) BEGIN
                             UPDATE [Innolux_DockDoor] SET DockDoorID=@DockDoorID, Remark=@Remark, Locate=@Locate, IP=@IP, CaptionPanelIP=@CaptionPanelIP, FilterCode=@FilterCode, Mask=@Mask, MaskOut=@MaskOut, UpdateTime=GetDate(), Updater=@Account WHERE ID=@ID;
                          END ELSE BEGIN
                             THROW 51000, '碼頭編號或IP已存在', 1;
                          END";
                }

                db.ExecuteSQL();
            }
        }

        /// <summary>儲存DockDoor InnoapStatus 資料</summary>
        public static void SaveDockDoorInfo_InnoapStatus(string IP, string InnoapStatus)
        {
            //開啟資料庫存取物件
            using (var db = new DataBase.DataBase(Definition.Conn))
            {
                //資料庫參數
                var param = new DataBase.SqlParams();
                param.Add("IP", IP);
                param.Add("InnoapStatus", InnoapStatus);

                db.SqlParams = param;

                //修改
                db.StrSQL =
                    @"DECLARE @Count int;
                          SET @Count = (SELECT COUNT(*) FROM [Innolux_DockDoor] WHERE IP = @IP);
                          IF (@Count = 1) BEGIN
                             UPDATE [Innolux_DockDoor] SET InnoapStatus=@InnoapStatus WHERE IP=@IP;
                          END ELSE BEGIN
                             THROW 51000, 'IP不存在', 1;
                          END";

                db.ExecuteSQL();
            }
        }
        /// <summary>取得白名單規則</summary>
        public static DataTable GetFilterList(string FilterCode)
        {
            //開啟資料庫存取物件
            using (var db = new DataBase.DataBase(Definition.Conn))
            {
                //資料庫參數
                var param = new DataBase.SqlParams();
                param.Add("FilterCode", FilterCode);
                db.SqlParams = param;

                //條件式
                string strWHERE = "";
                if (!string.IsNullOrEmpty(FilterCode)) strWHERE += " AND FilterCode = @FilterCode ";

                //取回資料
                db.StrSQL = "SELECT * FROM Innolux_Filter WHERE 1=1" + strWHERE;
                return db.ExecuteDataTable();
            }
        }

        /// <summary>移除白名單規則</summary>
        public static void RemoveFilter(string FilterCode)
        {
            //開啟資料庫存取物件
            using (var db = new DataBase.DataBase(Definition.Conn))
            {
                //資料庫參數
                var param = new DataBase.SqlParams();
                param.Add("FilterCode", FilterCode);
                db.SqlParams = param;

                //取回資料
                db.StrSQL =
                    @"DECLARE @Count int;
                      SET @Count = (SELECT COUNT(*) FROM Innolux_DockDoor WHERE FilterCode = @FilterCode);
                      IF(@Count = 0) BEGIN
                         DELETE FROM Innolux_Filter WHERE FilterCode = @FilterCode;
                      END ELSE BEGIN
                         THROW 51000, '無法刪除，已有碼頭使用此規則!', 1;
                      END";

                db.ExecuteSQL();
            }
        }

        /// <summary>儲存Filter資料</summary>
        public static void SaveFilter(Models.ViewModel.TagRule.TagRuleDetailViewModel m)
        {
            //驗証白名單字元
            string[] Rules = m.FilterRules.Split(',');
            int iRules = Rules.Count(x => x.Length == 2);
            if (iRules != Rules.Length) throw new Exception("白名單規則每組只允許2個字元");

            //開啟資料庫存取物件
            using (var db = new DataBase.DataBase(Definition.Conn))
            {
                //資料庫參數
                var param = new DataBase.SqlParams();
                param.Add("FilterCode", m.FilterCode);
                param.Add("FilterName", m.FilterName);
                param.Add("FilterRules", m.FilterRules);
                db.SqlParams = param;

                if (m.Action == "Add")
                {
                    //新增群組
                    db.StrSQL =
                        @"DECLARE @Count int;
                          SET @Count = (SELECT COUNT(*) FROM Innolux_Filter WHERE FilterCode = @FilterCode);
                          IF (@Count = 0) BEGIN
                             INSERT Innolux_Filter(FilterCode, FilterName, FilterRules) VALUES(@FilterCode, @FilterName, @FilterRules);
                          END ELSE BEGIN
                             THROW 51000, '規則編號已存在', 1;
                          END";
                }
                else
                {
                    //修改群組
                    db.StrSQL = "UPDATE Innolux_Filter SET FilterName=@FilterName, FilterRules=@FilterRules WHERE FilterCode=@FilterCode";
                }

                db.ExecuteSQL();
            }
        }

        /// <summary>更新貨櫃的作業狀態</summary>
        public static string UpdateTruckStatus(string IP, out string ContainerStatus)
        {
            string strContainerID = "", strMsg = "";

            //取得DockDoor資訊
            DataTable dtDoor = GetDockDoorList(null, IP, null, null, new Pages());
            DataRow row = dtDoor.Rows[0];

            //取得貨櫃狀態
            strContainerID = row["ContainerID"].ToString();
            ContainerStatus = row["ContainerStatus"].ToString();
            if (strContainerID != "")
            {
                //取得並更新貨櫃狀態
                ContainerStatus = ApiDataAccess.GetContainerStatus(strContainerID);
                using (var db = new DataBase.DataBase(Definition.Conn))
                {
                    db.StrSQL = "UPDATE Innolux_DockDoor SET ContainerStatus = '" + ContainerStatus + "' WHERE IP = '" + IP + "'";
                    db.ExecuteSQL();
                }

                //更新控制器的貨櫃狀態
                ApiDataAccess.UpdateControlDB(IP, "UPDATE status SET status = '" + ContainerStatus + "'");

                //依狀態回傳結果
                if (ContainerStatus == "O")
                    strMsg = string.Format("貨櫃號碼{0}的狀態為：{1}", strContainerID, "待裝櫃");
                else if (ContainerStatus == "I")
                    strMsg = string.Format("貨櫃號碼{0}的狀態為：{1}", strContainerID, "待卸櫃");
                else
                    throw new Exception("無" + strContainerID + "的貨櫃狀態");
            }
            else if(ContainerStatus == "N")
            {
                throw new Exception("無貨櫃標籤");
            }
            else
            {
                throw new Exception("無貨櫃靠站資訊");
            }

            return strMsg;
        }
    }
}