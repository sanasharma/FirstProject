using System;
using System.Reflection;
using System.Collections.Generic;
using System.Data;
using System.Web;
using Models.Model;

namespace EPC
{
    public class Tool
    {
        /// <summary>檢查選單權限</summary>
        public static bool CheckMenuAuthority(string MenuNo)
        {
            bool flag = false;
            var ui = Definition.UserInfo;
            var auth = Definition.UserAuthority;

            if (ui.IsSuper)
            {
                flag = true;
            }
            else
            {
                flag = auth.Exists(x => x.MenuNo == MenuNo);
            }

            return flag;
        }

        /// <summary>設定頁面細部權限</summary>
        public static Auth GetPageAuthority()
        {
            User ui = Definition.UserInfo;
            Auth auth = new Auth();

            //若不為管理者，則設定登入者本頁的頁面權限
            if (!ui.IsSuper)
            {
                string strMenuNo = Definition.SelectMenuNo;
                auth = Definition.UserAuthority.Find(x => x.MenuNo == strMenuNo);
            }

            return auth;
        }
    }
}