using System;
using System.Data;
using System.Web;
using Models.Base;
using Models.ViewModel.DockDoor;
using MySql.Data.MySqlClient;
using Tool;

namespace DataAccess
{
    public class TagLogDataAccess
    {
        /// <summary>取得TagLog資料</summary>
        public static DataTable GetTagLogList(string DockDoorID, string Status, Pages _page)
        {
            //開啟資料庫存取物件
            using (var db = new DataBase.DataBase(Definition.Conn))
            {
                //判斷Status成功或失敗
                Status = (Status == "Y" ? "'Y','A'" : Status);
                Status = (Status == "N" ? "'N','0'" : Status);

                //資料庫參數
                var param = new DataBase.SqlParams();
                param.Add("DockDoorID", DockDoorID);
                db.SqlParams = param;

                //條件式
                string strWHERE = "";
                if (!string.IsNullOrEmpty(DockDoorID)) strWHERE += " AND DockDoorID = @DockDoorID ";
                if (!string.IsNullOrEmpty(Status)) strWHERE += " AND Status IN (" + Status + ") ";

                //取回資料
                int iTotal;
                var sql = "SELECT * FROM Innolux_TagLog WHERE 1=1" + strWHERE;
                var dt = db.ExecuteDataTable(sql, "ID DESC", _page.PageIndex, _page.PageSize, out iTotal);
                _page.TotalRows = iTotal;

                return dt;
            }
        }
    }
}