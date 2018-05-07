using System;
using System.Data;
using Models.Base;
using Models.ViewModel.SysParams;

namespace DataAccess
{
    public class SysParamsDataAccess
    {
        /// <summary>取得系統參數資料</summary>
        public static DataTable GetSysParams(string ParaCode, string ParaDesc, Pages _page)
        {
            //開啟資料庫存取物件
            using (var db = new DataBase.DataBase(Definition.Conn))
            {
                //資料庫參數
                var param = new DataBase.SqlParams();
                param.Add("ParaCode", ParaCode);
                param.Add("ParaDesc", "%" + ParaDesc + "%");
                db.SqlParams = param;

                //條件式
                string strWHERE = "";
                if (!string.IsNullOrEmpty(ParaCode)) strWHERE += " AND ParaCode = @ParaCode ";
                if (!string.IsNullOrEmpty(ParaDesc)) strWHERE += " AND ParaDesc LIKE @ParaDesc ";

                //取回資料
                int iTotal;
                var sql = "SELECT * FROM [SysParams] WHERE 1=1" + strWHERE;
                var dt = db.ExecuteDataTable(sql, "ParaCode", _page.PageIndex, _page.PageSize, out iTotal);
                _page.TotalRows = iTotal;

                return dt;
            }
        }

        /// <summary>儲存系統參數資料</summary>
        public static bool SaveSysParamsInfo(SysParamsDetailViewModel m)
        {
            //開啟資料庫存取物件
            using (var db = new DataBase.DataBase(Definition.Conn))
            {
                //資料庫參數
                var param = new DataBase.SqlParams();
                param.Add("ParaCode", m.ParaCode);
                param.Add("ParaValue", m.ParaValue);
                param.Add("ParaDesc", m.ParaDesc);
                db.SqlParams = param;

                if (m.Action == "Add")
                {
                    //驗証系統代碼是否重覆
                    var dtParams = GetSysParams(m.ParaCode, null, new Pages());
                    if(dtParams.Rows.Count > 0)
                    {
                        throw new Exception("已有重覆的系統代碼");
                    }

                    //新增系統參數
                    db.StrSQL = "INSERT [SysParams](ParaCode, ParaValue, ParaDesc) VALUES(@ParaCode, @ParaValue, @ParaDesc);";
                }
                else
                {
                    //修改系統參數
                    db.StrSQL = "UPDATE [SysParams] SET ParaValue = @ParaValue, ParaDesc = @ParaDesc WHERE ParaCode = @ParaCode;";
                }

                int iResult = db.ExecuteSQL();
                return (iResult > 0);
            }
        }

        /// <summary>取得系統參數值</summary>
        public static string GetSysParamsValue(string ParaCode)
        {
            //開啟資料庫存取物件
            using (var db = new DataBase.DataBase(Definition.Conn))
            {
                //資料庫參數
                var param = new DataBase.SqlParams();
                param.Add("ParaCode", ParaCode);
                db.SqlParams = param;

                //回傳資料
                db.StrSQL = "SELECT ParaValue FROM [SysParams] WHERE ParaCode = @ParaCode";
                return db.ExecuteScalar();
            }
        }
    }
}
