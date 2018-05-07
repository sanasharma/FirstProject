using System;
using System.Data;
using Models.Base;
using Models.ViewModel.Password;
using Tool;

namespace DataAccess
{
    public class PasswordDataAccess
    {
        /// <summary>修改使用者密碼</summary>
        public static bool EditPassword(PasswordEditViewModel m)
        {
            //找出此使用者資訊
            DataTable dt = UserDataAccess.GetUserList(m.ID, null, null, null, null, new Pages());
            if (dt.Rows.Count > 0)
            {
                //驗証
                string strOldPwd = dt.Rows[0]["Password"].ToString();
                string strOldPwd1 = Util.MD5(m.Password, 32);
                if (strOldPwd1 != strOldPwd) throw new Exception("原密碼不相符");

                //開啟資料庫存取物件
                using (var db = new DataBase.DataBase(Definition.Conn))
                {
                    //資料庫參數
                    var param = new DataBase.SqlParams();
                    param.Add("ID", m.ID);
                    param.Add("NewPassword", Util.MD5(m.NewPassword, 32));
                    db.SqlParams = param;

                    //異動資料
                    db.StrSQL = "UPDATE [User] SET Password = @NewPassword WHERE ID = @ID";
                    int iResult = db.ExecuteSQL();
                    return (iResult > 0);
                }
            }
            else
            {
                throw new Exception("無此使用者相關資訊");
            }
        }
    }
}