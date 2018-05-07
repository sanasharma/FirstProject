using System;
using System.Data;
using Models.Base;
using Models.ViewModel;
using Tool;

namespace DataAccess
{
    public class UserDataAccess
    {
        /// <summary>取得使用者清單資料檔</summary>
        public static DataTable GetUserList(int? _id, string _account, string _password, string _email, string _name, Pages _page)
        {
            //開啟資料庫存取物件
            using (var db = new DataBase.DataBase(Definition.Conn))
            {
                //資料庫參數
                var param = new DataBase.SqlParams();
                param.Add("ID", _id);
                param.Add("Account", _account);
                param.Add("Password", _password);
                param.Add("Email", "%" + _email + "%");
                param.Add("Name", "%" + _name + "%");
                db.SqlParams = param;

                //條件式
                string strWhere = "";
                if (_id != null) { strWhere += " AND ID = @ID "; }
                if (!string.IsNullOrEmpty(_account)) { strWhere += " AND Account = @Account "; }
                if (!string.IsNullOrEmpty(_password)) { strWhere += " AND Password = @Password "; }
                if (!string.IsNullOrEmpty(_email)) { strWhere += " AND Email LIKE @Email "; }
                if (!string.IsNullOrEmpty(_name)) { strWhere += " AND Name LIKE @Name "; }

                //取回資料
                int iTotal;
                var sql = "SELECT * FROM [User] WHERE 1=1 " + strWhere;
                var dt = db.ExecuteDataTable(sql, "ID", _page.PageIndex, _page.PageSize, out iTotal);
                _page.TotalRows = iTotal;

                return dt;
            }
        }

        /// <summary>取得使用者權限檔</summary>
        public static DataTable GetUserAuthority(string _account)
        {
            //開啟資料庫存取物件
            using (var db = new DataBase.DataBase(Definition.Conn))
            {
                //資料庫參數
                var param = new DataBase.SqlParams();
                param.Add("Account", _account);
                param.Add("Enabled", true);
                db.SqlParams = param;

                //取回資料
                db.StrSQL = @"SELECT MenuNo,
	                            Convert(bit, MAX(Convert(int, [Add]))) [Add],
	                            Convert(bit, MAX(Convert(int, [Edit]))) [Edit],
	                            Convert(bit, MAX(Convert(int, [Del]))) [Del],	
	                            Convert(bit, MAX(Convert(int, [Query]))) [Query],	
	                            Convert(bit, MAX(Convert(int, [Audit]))) [Audit],
	                            Convert(bit, MAX(Convert(int, [Print]))) [Print],
	                            Convert(bit, MAX(Convert(int, [Export]))) [Export],
	                            Convert(bit, MAX(Convert(int, [Import]))) [Import],
	                            Convert(bit, MAX(Convert(int, [Admin]))) [Admin],
	                            Convert(bit, MAX(Convert(int, [Enabled]))) [Enabled]
                              FROM(
	                            SELECT a.* FROM [Auth] a WHERE a.Account = @Account AND a.Enabled = @Enabled
	                            UNION ALL
	                            SELECT a.* FROM [Auth] a, [GroupUser] g WHERE a.GroupID = g.GroupID AND a.Enabled = @Enabled AND g.Account = @Account
                              ) r
                              GROUP BY MenuNo";
                return db.ExecuteDataTable();
            }
        }

        /// <summary>修改使用者資料</summary>
        public static void SaveUserInfo(Models.ViewModel.User.UserDetailViewModel m)
        {
            string action = "";

            //新增
            if (m.ID == 0)
            {
                action = "Add";
                m.Password = SysParamsDataAccess.GetSysParamsValue("DefaultUserPassword"); //預設密碼

                //驗証帳號是否重覆
                var dt = GetUserList(null, m.Account, null, null, null, new Pages());
                if (dt.Rows.Count > 0)
                {
                    throw new Exception("已有重覆的帳號");
                }
            }

            //開啟資料庫存取物件
            using (var db = new DataBase.DataBase(Definition.Conn, true))
            {
                try
                {
                    //資料庫參數
                    var param = new DataBase.SqlParams();
                    param.Add("ID", m.ID);
                    param.Add("Email", m.Email);
                    param.Add("Password", Util.MD5(m.Password, 32));
                    param.Add("Account", m.Account);
                    param.Add("Name", m.Name);
                    param.Add("IsSuper", m.IsSuper);
                    param.Add("IsLock", m.IsLock);
                    param.Add("LastLoginDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    param.Add("CreateDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    db.SqlParams = param;

                    if (action == "Add")
                    {
                        //新增使用者
                        db.StrSQL = "INSERT [User](Email, Password, Account, Name, IsSuper, IsLock, LastLoginDate, CreateDate) VALUES(@Email, @Password, @Account, @Name, @IsSuper, @IsLock, @LastLoginDate, @CreateDate);";
                    }
                    else
                    {
                        //修改使用者
                        db.StrSQL = "UPDATE [User] SET Email=@Email, Name=@Name, IsSuper=@IsSuper, IsLock=@IsLock WHERE ID=@ID;";
                    }

                    //刪除舊有使用者選單權限與群組資料
                    db.StrSQL += @"DELETE FROM [Auth] WHERE Account = @Account;
                                   DELETE FROM [GroupUser] WHERE Account = @Account";
                    db.ExecuteSQL();

                    //逐筆判斷選單權限
                    foreach (var ma in m.AuthList)
                    {
                        if (ma.Add || ma.Edit || ma.Del || ma.Query || ma.Audit || ma.Print || ma.Export || ma.Import || ma.Admin)
                        {
                            param.Clear();
                            param.Add("Account", m.Account);
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
                                           VALUES('', @Account, @MenuNo, @Add, @Edit, @Del, @Query, @Audit, @Print, @Export, @Import, @Admin, @Enabled, @CreateDate, @ModifyDate);";
                            db.ExecuteSQL();
                        }
                    }

                    //逐筆新增使用者所屬的群組
                    foreach (var g in m.GroupItems)
                    {
                        param.Clear();
                        param.Add("GroupID", g.GroupID);
                        param.Add("Account", m.Account);
                        db.SqlParams = param;

                        db.StrSQL = "INSERT [GroupUser](GroupID, Account) VALUES(@GroupID, @Account)";
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

        /// <summary>更新使用者登入時間</summary>
        public static void SetUserLoginDate(LoginViewModel m)
        {
            //開啟資料庫存取物件
            using (var db = new DataBase.DataBase(Definition.Conn, true))
            {
                try
                {
                    //資料庫參數
                    var param = new DataBase.SqlParams();
                    param.Add("Account", m.Account);
                    param.Add("IP", m.IP);
                    param.Add("LastLoginDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    db.SqlParams = param;

                    //更新使用者最後登入時間，記錄使用者登錄Log
                    db.StrSQL = @"UPDATE [User] SET LastLoginDate = @LastLoginDate WHERE Account = @Account;
                                  INSERT [UserLoginLog](Account, IP, LoginDate) VALUES(@Account, @IP, @LastLoginDate);";
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

        /// <summary>還原使用者密碼為預設密碼</summary>
        public static bool SetUserDefaultPassword(string Account)
        {
            //開啟資料庫存取物件
            using (var db = new DataBase.DataBase(Definition.Conn))
            {
                //預設密碼
                string pwd = SysParamsDataAccess.GetSysParamsValue("DefaultUserPassword");

                //資料庫參數
                var param = new DataBase.SqlParams();
                param.Add("Account", Account);
                param.Add("Password", Util.MD5(pwd, 32));
                db.SqlParams = param;

                //更新使用者最後登入時間
                db.StrSQL = "UPDATE [User] SET Password = @Password WHERE Account = @Account";
                int iResult = db.ExecuteSQL();
                return (iResult > 0);
            }
        }

        /// <summary>取得使用者的選單權限</summary>
        public static DataTable GetUserMenuAuth(string Account)
        {
            //開啟資料庫存取物件
            using (var db = new DataBase.DataBase(Definition.Conn))
            {
                //資料庫參數
                var param = new DataBase.SqlParams();
                param.Add("Account", Account);
                db.SqlParams = param;

                //取回資料
                db.StrSQL = @"SELECT m.MenuNo, m.MenuName, m.MenuIco, m.OrderID, a.* FROM Menu m
                              LEFT OUTER JOIN Auth a ON a.MenuNo = m.MenuNo AND a.Account = @Account
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
    }
}