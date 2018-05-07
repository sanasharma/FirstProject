using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Data;
using Models.ViewModel.Menu;
using Tool;

namespace DataAccess
{
    public class MenuDataAccess
    {
        /// <summary>取得所有選單檔</summary>
        public static DataTable GetAllMenuList(bool? Enabled)
        {
            //開啟資料庫存取物件
            using (var db = new DataBase.DataBase(Definition.Conn))
            {
                //資料庫參數
                var param = new DataBase.SqlParams();
                param.Add("Enabled", Enabled);
                db.SqlParams = param;

                //條件式
                string strWHERE = "";
                if (Enabled != null) strWHERE += " AND Enabled = @Enabled ";

                //取回資料
                db.StrSQL = "SELECT * FROM Menu WHERE 1=1" + strWHERE + "ORDER BY OrderID";
                var dt = db.ExecuteDataTable();
                var dtNew = dt.Clone();

                //依選單順序排列
                foreach (var r in dt.Select("LEN(MenuNo) = 2", "OrderID"))
                {
                    dtNew.Rows.Add(r.ItemArray);
                    foreach (var r1 in dt.Select("LEN(MenuNo) = 4 AND MenuNo LIKE '" + r["MenuNo"].ToString() + "%'"))
                    {
                        dtNew.Rows.Add(r1.ItemArray);
                    }
                }

                return dtNew;
            }
        }

        /// <summary>取得選單檔</summary>
        public static DataTable GetMenuList(int? MenuNoLen, string MenuNo, string MenuName, bool? Enabled)
        {
            //開啟資料庫存取物件
            using (var db = new DataBase.DataBase(Definition.Conn))
            {
                //資料庫參數
                var param = new DataBase.SqlParams();
                param.Add("MenuNoLen", MenuNoLen);
                param.Add("MenuNo", MenuNo);
                param.Add("MenuName", MenuName);
                param.Add("Enabled", Enabled);
                db.SqlParams = param;

                //條件式
                string strWhere = "";
                if (MenuNoLen != null) { strWhere += " AND LEN(MenuNo) = @MenuNoLen "; }
                if (!string.IsNullOrEmpty(MenuNo)) { strWhere += " AND MenuNo = @MenuNo "; }
                if (!string.IsNullOrEmpty(MenuName)) { strWhere += " AND MenuName = @MenuName "; }
                if (Enabled != null) { strWhere += " AND Enabled = @Enabled "; }

                //取回資料
                db.StrSQL = "SELECT * FROM Menu WHERE 1=1" + strWhere + " ORDER BY OrderID";
                return db.ExecuteDataTable();
            }
        }

        /// <summary>儲存選單資料</summary>
        public static bool SaveMenuInfo(MenuDetailViewModel m)
        {
            string action = "";

            //有無選單編號，若無則視為新增
            if (string.IsNullOrEmpty(m.MenuNo))
            {
                action = "Add";

                //判斷是否為空值
                m.ParentMenuNo = string.IsNullOrWhiteSpace(m.ParentMenuNo) ? "" : m.ParentMenuNo;

                //設定新選單編號
                string strMaxMenuNo = MenuDataAccess.GetMaxMenuNo(m.ParentMenuNo, m.ParentMenuNo.Length + 2);
                m.MenuNo = Util.GetPadLeftString(strMaxMenuNo, 1);
            }

            //開啟資料庫存取物件
            using (var db = new DataBase.DataBase(Definition.Conn))
            {
                //資料庫參數
                var param = new DataBase.SqlParams();
                param.Add("MenuNo", m.MenuNo);
                param.Add("MenuName", m.MenuName);
                param.Add("MenuLink", m.MenuLink);
                param.Add("Type", m.Type);
                param.Add("MenuDesc", m.MenuDesc);
                param.Add("OrderID", m.OrderID);
                param.Add("Enabled", m.Enabled);
                param.Add("MenuIco", m.MenuIco);
                db.SqlParams = param;

                if (action == "Add")
                {
                    //新增資料
                    db.StrSQL = @"--設定排序值--
                    SET @OrderID = (SELECT MAX(OrderID) + 1 FROM Menu WHERE LEN(MenuNo) = LEN(@MenuNo));
                    INSERT Menu(MenuNo, MenuName, MenuLink, Type, MenuDesc, OrderID, Enabled, MenuIco) VALUES(@MenuNo, @MenuName, @MenuLink, @Type, @MenuDesc, @OrderID, @Enabled, @MenuIco)";
                }
                else
                {
                    db.StrSQL = "UPDATE Menu SET MenuName=@MenuName, MenuLink=@MenuLink, Type=@Type, MenuDesc=@MenuDesc, OrderID=@OrderID, Enabled=@Enabled, MenuIco=@MenuIco WHERE MenuNo=@MenuNo";
                }

                int iResult = db.ExecuteSQL();
                return (iResult > 0);
            }
        }

        /// <summary>取得條件式最後的選單編號</summary>
        public static string GetMaxMenuNo(string ParentMenuNo, int? MenuNoLen)
        {
            //開啟資料庫存取物件
            using (var db = new DataBase.DataBase(Definition.Conn))
            {
                //資料庫參數
                var param = new DataBase.SqlParams();
                param.Add("ParentMenuNo", ParentMenuNo + "%");
                param.Add("MenuNoLen", MenuNoLen);
                db.SqlParams = param;

                //條件式
                string strWhere = "";
                if (!string.IsNullOrEmpty(ParentMenuNo)) { strWhere += " AND MenuNo LIKE @ParentMenuNo "; }
                if (MenuNoLen != null) { strWhere += " AND LEN(MenuNo) = @MenuNoLen "; }

                //取回資料
                db.StrSQL = "SELECT MAX(MenuNo) FROM Menu WHERE 1=1" + strWhere;

                //編號轉換，若回傳空值則從00編號開始
                string strMaxMenuNo = db.ExecuteScalar();
                if(string.IsNullOrEmpty(strMaxMenuNo))
                {
                    strMaxMenuNo = ParentMenuNo + "00";
                }
                
                return strMaxMenuNo;
            }
        }

        /// <summary>刪除指定的選單編號</summary>
        public static void DelMenu(string MenuNo)
        {
            //開啟資料庫存取物件
            using (var db = new DataBase.DataBase(Definition.Conn))
            {
                //資料庫參數
                var param = new DataBase.SqlParams();
                param.Add("MenuNo", MenuNo + "%");
                db.SqlParams = param;

                //刪除相關資料表
                db.StrSQL =
                    @"DECLARE @Count int;
                      SET @Count = (SELECT COUNT(*) FROM [Menu] WHERE MenuNo LIKE @MenuNo);

                      IF (@Count = 1) BEGIN
                        DELETE FROM [Menu] WHERE MenuNo LIKE @MenuNo;
                        DELETE FROM [Auth] WHERE MenuNo LIKE @MenuNo;
                      END ELSE BEGIN
                        THROW 51000, '無法刪除子選單!', 1;
                      END";

                db.ExecuteSQL();
            }
        }
    }
}