using System;
using System.Data;
using System.Web;
using Models.Base;
using Models.ViewModel.DockDoor;
using MySql.Data.MySqlClient;
using Tool;

namespace DataAccess
{
    public class FilterDataAccess
    {
        /// <summary>取得TagFilter資料</summary>
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
                db.StrSQL = "SELECT * FROM [Innolux_Filter] WHERE 1=1" + strWHERE;
                DataTable dt = db.ExecuteDataTable();

                return dt;
            }
        }
    }
}