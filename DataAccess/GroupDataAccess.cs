using System;
using System.Data;
using Models.Base;
using Models.ViewModel.Group;
using Tool;

namespace DataAccess
{
    public class GroupDataAccess
    {
        /// <summary>取得群組資料</summary>
        public static DataTable GetGroupList(string GroupID, string GroupName, Pages _page)
        {
            //開啟資料庫存取物件
            using (var db = new DataBase.DataBase(Definition.Conn))
            {
                //資料庫參數
                var param = new DataBase.SqlParams();
                param.Add("GroupID", GroupID);
                param.Add("GroupName", "%" + GroupName + "%");
                db.SqlParams = param;

                //條件式
                string strWHERE = "";
                if (!string.IsNullOrEmpty(GroupID)) strWHERE += " AND GroupID = @GroupID ";
                if (!string.IsNullOrEmpty(GroupName)) strWHERE += " AND GroupName LIKE @GroupName ";

                //取回資料
                int iTotal;
                var sql = "SELECT * FROM [Group] WHERE 1=1" + strWHERE;
                var dt = db.ExecuteDataTable(sql, "GroupID", _page.PageIndex, _page.PageSize, out iTotal);
                _page.TotalRows = iTotal;

                return dt;
            }
        }

        /// <summary>取得條件式最後的群組編號</summary>
        public static string GetMaxGroupID()
        {
            //開啟資料庫存取物件
            using (var db = new DataBase.DataBase(Definition.Conn))
            {
                //取回資料
                db.StrSQL = "SELECT MAX(GroupID) FROM [Group]";
                string strID = db.ExecuteScalar();

                return (strID == "" ? "00" : strID);
            }
        }

        /// <summary>儲存群組資料</summary>
        public static void SaveGroupInfo(GroupDetailViewModel m)
        {
            string action = "";

            //有無群組編號，若無則視為新增
            if (string.IsNullOrEmpty(m.GroupID))
            {
                action = "Add";

                //設定新群組編號
                string strMaxGroupID = GroupDataAccess.GetMaxGroupID();
                m.GroupID = Util.GetPadLeftString(strMaxGroupID, 1);
            }

            //開啟資料庫存取物件
            using (var db = new DataBase.DataBase(Definition.Conn, true))
            {
                try
                {
                    //資料庫參數
                    var param = new DataBase.SqlParams();
                    param.Add("GroupID", m.GroupID);
                    param.Add("GroupName", m.GroupName);
                    param.Add("GroupDesc", m.GroupDesc);
                    db.SqlParams = param;

                    if (action == "Add")
                    {
                        //新增群組
                        db.StrSQL = "INSERT [Group](GroupID, GroupName, GroupDesc) VALUES(@GroupID, @GroupName, @GroupDesc);";
                    }
                    else
                    {
                        //修改群組
                        db.StrSQL = "UPDATE [Group] SET GroupName=@GroupName, GroupDesc=@GroupDesc WHERE GroupID=@GroupID;";
                    }

                    //刪除舊有群組選單權限與使用者清單
                    db.StrSQL += @"DELETE FROM [Auth] WHERE GroupID = @GroupID;
                                   DELETE FROM [GroupUser] WHERE GroupID = @GroupID;";
                    db.ExecuteSQL();

                    //逐筆判斷選單權限
                    foreach (var ma in m.AuthList)
                    {
                        if (ma.Add || ma.Edit || ma.Del || ma.Query || ma.Audit || ma.Print || ma.Export || ma.Import || ma.Admin)
                        {
                            param.Clear();
                            param.Add("GroupID", m.GroupID);
                            param.Add("MenuNo", ma.MenuNo);
                            param.Add("Add", ma.Add);
                            param.Add("Edit", ma.Edit);
                            param.Add("Del", ma.Del);
                            param.Add("Query", ma.Query);
                            param.Add("Audit", ma.Audit);
                            param.Add("Print", ma.Print);
                            param.Add("Export", ma.Export);
                            param.Add("Import", ma.Import);
                            param.Add("Admin", ma.Admin);
                            param.Add("Enabled", true);
                            param.Add("CreateDate", DateTime.Now);
                            param.Add("ModifyDate", DateTime.Now);
                            db.SqlParams = param;

                            db.StrSQL = @"INSERT [Auth](GroupID, Account, MenuNo, [Add], Edit, Del, Query, Audit, [Print], Export, Import, Admin, Enabled, CreateDate, ModifyDate) 
                                           VALUES(@GroupID, '', @MenuNo, @Add, @Edit, @Del, @Query, @Audit, @Print, @Export, @Import, @Admin, @Enabled, @CreateDate, @ModifyDate);";
                            db.ExecuteSQL();
                        }
                    }

                    //逐筆新增群組底下的使用者
                    foreach (var u in m.GroupUserItems)
                    {
                        param.Clear();
                        param.Add("GroupID", m.GroupID);
                        param.Add("Account", u.Account);
                        db.SqlParams = param;

                        db.StrSQL = @"INSERT [GroupUser](GroupID, Account) VALUES(@GroupID, @Account);";
                        db.ExecuteSQL();
                    }

                    //確認執行交易
                    db.Commit();
                }
                catch (Exception e)
                {
                    //回復交易
                    db.Rollback();
                    throw e;
                }
            }
        }

        /// <summary>取得群組的選單權限</summary>
        public static DataTable GetGroupMenuAuth(string GroupID)
        {
            //開啟資料庫存取物件
            using (var db = new DataBase.DataBase(Definition.Conn))
            {
                //資料庫參數
                var param = new DataBase.SqlParams();
                param.Add("GroupID", GroupID);
                db.SqlParams = param;

                //取回資料
                db.StrSQL = @"SELECT m.MenuNo, m.MenuName, m.MenuIco, m.OrderID, a.* FROM Menu m
                              LEFT OUTER JOIN Auth a ON a.MenuNo = m.MenuNo AND a.GroupID = @GroupID
                              ORDER BY OrderID";

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

        /// <summary>取得使用者所屬的群組清單</summary>
        public static DataTable GetUserGroup(string Account)
        {
            //開啟資料庫存取物件
            using (var db = new DataBase.DataBase(Definition.Conn))
            {
                //資料庫參數
                var param = new DataBase.SqlParams();
                param.Add("Account", Account);
                db.SqlParams = param;

                //取回資料
                db.StrSQL = @"SELECT g.* FROM GroupUser gu
                              INNER JOIN [Group] g ON g.GroupID = gu.GroupID 
                              WHERE Account = @Account";

                var dt = db.ExecuteDataTable();
                return dt;
            }
        }

        /// <summary>取得群組底下的使用者清單</summary>
        public static DataTable GetGroupUser(string GroupID)
        {
            //開啟資料庫存取物件
            using (var db = new DataBase.DataBase(Definition.Conn))
            {
                //資料庫參數
                var param = new DataBase.SqlParams();
                param.Add("GroupID", GroupID);
                db.SqlParams = param;

                //取回資料
                db.StrSQL = @"SELECT u.* FROM GroupUser gu
                              INNER JOIN [User] u ON u.Account = gu.Account 
                              WHERE gu.GroupID = @GroupID";

                var dt = db.ExecuteDataTable();
                return dt;
            }
        }

        /// <summary>刪除群組</summary>
        public static void RemoveGroup(string GroupID)
        {
            //開啟資料庫存取物件
            using (var db = new DataBase.DataBase(Definition.Conn, true))
            {
                try
                {
                    //資料庫參數
                    var param = new DataBase.SqlParams();
                    param.Add("GroupID", GroupID);
                    db.SqlParams = param;

                    //刪除舊有群組選單權限與使用者清單
                    db.StrSQL += @"DELETE FROM [Auth] WHERE GroupID = @GroupID;
                                   DELETE FROM [GroupUser] WHERE GroupID = @GroupID;
                                   DELETE FROM [Group] WHERE GroupID = @GroupID;";
                    db.ExecuteSQL();

                    //確認執行交易
                    db.Commit();
                }
                catch (Exception e)
                {
                    //回復交易
                    db.Rollback();
                    throw e;
                }
            }
        }
    }
}
