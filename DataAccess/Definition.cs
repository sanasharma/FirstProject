using System;
using System.Data;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using Models.Model;

namespace DataAccess
{
    public class Definition
    {
        /// <summary>紀錄碼頭最近上報時間</summary>
        public static DateTime LastPostTime = DateTime.Now;

        /// <summary>紀錄相近時間上報的貨櫃清單</summary>
        public static List<string> CONTAINERS = new List<string>();

        /// <summary>紀錄字幕機更新時間</summary>
        public static DateTime LastPanelUpdateTime = DateTime.Now;

        /// <summary>conn連線字串</summary>
        public static string Conn
        {
            get { return ConfigurationManager.ConnectionStrings["conn"].ConnectionString; }
        }

        /// <summary>控制器conn連線字串</summary>
        public static string ControlConn
        {
            get { return ConfigurationManager.ConnectionStrings["control_conn"].ConnectionString; }
        }

        /// <summary>Oracle WMS連線字串</summary>
        public static string WMSConn
        {
            get { return ConfigurationManager.ConnectionStrings["wms_conn"].ConnectionString; }
        }

        /// <summary>使用者登入資訊檔</summary>
        public static User UserInfo
        {
            get
            {
                if (HttpContext.Current.Session["UserInfo"] != null)
                    return (User)HttpContext.Current.Session["UserInfo"];

                return null;
            }
        }

        /// <summary>廠別代碼</summary>
        public static string AppPlant
        {
            get
            {
                return System.Web.Configuration.WebConfigurationManager.AppSettings["AppPlant"];
            }
        }
    }
}