using System;
using System.Data;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using Models.Model;

namespace EPC
{
    public class Definition
    {
        /// <summary>使用者登入資訊檔</summary>
        public static User UserInfo
        {
            set
            {
                HttpContext.Current.Session["UserInfo"] = value;
            }
            get
            {
                if (HttpContext.Current.Session["UserInfo"] != null)
                    return (User)HttpContext.Current.Session["UserInfo"];

                return null;
            }
        }

        /// <summary>使用者選單檔</summary>
        public static List<Menu> MenuList
        {
            set
            {
                HttpContext.Current.Session["UserMenuList"] = value;
            }
            get
            {
                if (HttpContext.Current.Session["UserMenuList"] != null)
                    return (List<Menu>)HttpContext.Current.Session["UserMenuList"];

                return null;
            }
        }

        /// <summary>使用者選單頁面權限檔</summary>
        public static List<Auth> UserAuthority
        {
            set
            {
                HttpContext.Current.Session["UserAuthority"] = value;
            }
            get
            {
                return (List<Auth>)HttpContext.Current.Session["UserAuthority"];
            }
        }

        /// <summary>記錄選擇的選單</summary>
        public static string SelectMenuNo
        {
            set
            {
                HttpContext.Current.Session["SelectMenuNo"] = value;
            }
            get
            {
                if (HttpContext.Current.Session["SelectMenuNo"] != null)
                    return (string)HttpContext.Current.Session["SelectMenuNo"];

                return "00";
            }
        }

        /// <summary>應用程式名稱</summary>
        public static string AppTitle
        {
            get
            {
                return System.Web.Configuration.WebConfigurationManager.AppSettings["AppTitle"];
            }
        }
    }
}