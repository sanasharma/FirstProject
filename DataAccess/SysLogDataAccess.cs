using System;
using System.Data;
using Models.Base;
using Models.ViewModel;
using Tool;

namespace DataAccess
{
    public class SysLogDataAccess
    {
        /// <summary>取得使用者清單資料檔</summary>
        public static DataTable GetUserLoginLog(string _account, string _ip, Pages _page)
        {
            //開啟資料庫存取物件
            using (var db = new DataBase.DataBase(Definition.Conn))
            {
                //資料庫參數
                var param = new DataBase.SqlParams();
                param.Add("Account", _account);
                param.Add("IP", _ip);
                db.SqlParams = param;

                //條件式
                string strWhere = "";
                if (!string.IsNullOrEmpty(_account)) { strWhere += " AND Account = @Account "; }
                if (!string.IsNullOrEmpty(_ip)) { strWhere += " AND IP = @IP "; }

                //取回資料
                int iTotal;
                var sql = "SELECT * FROM [UserLoginLog] WHERE 1=1 " + strWhere;
                var dt = db.ExecuteDataTable(sql, "LoginDate DESC", _page.PageIndex, _page.PageSize, out iTotal);
                _page.TotalRows = iTotal;

                return dt;
            }
        }
    }
}