using System;
using System.Xml;
using System.Reflection;
using System.Collections.Generic;
using System.Data;
using System.Web;
using System.Net;
using System.Net.NetworkInformation;
using System.Xml.Serialization;
using System.IO;
using Newtonsoft.Json;
using System.Text;

namespace Tool
{
    public class Util
    {
        /// <summary>DataTable轉換為泛型集合</summary>
        public static List<T> ToList<T>(DataTable dt)
        {
            List<T> result = new List<T>();
            for (int j = 0; j < dt.Rows.Count; j++)
            {
                T _t = (T)Activator.CreateInstance(typeof(T));
                PropertyInfo[] propertys = _t.GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        //属性與字段名稱一致就進行賦值                           
                        if (pi.Name.Equals(dt.Columns[i].ColumnName))
                        {
                            //DB NULL值單獨處理                               
                            if (dt.Rows[j][i] != DBNull.Value)
                                pi.SetValue(_t, dt.Rows[j][i], null);
                            else
                                pi.SetValue(_t, null, null);
                            break;
                        }
                    }
                }
                result.Add(_t);
            }
            return result;
        }

        /// <summary>取得累加數字的文字內容補0</summary>
        public static string GetPadLeftString(string value, int iNumber)
        {
            int iCount = value.Length;
            int iVal = int.Parse(value) + iNumber;
            return iVal.ToString().PadLeft(iCount, '0');
        }

        /// <summary>MD5公用函式</summary>
        public static string MD5(string str, int code)
        {
            if (code == 16)
            {
                return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5").ToLower().Substring(8, 16);
            }
            else if (code == 32)
            {
                return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5").ToLower();
            }

            return "00000000000000000000000000000000";
        }

        /// <summary>取得正確的Client端IP</summary>
        public static string GetClientIP()
        {
            var r = HttpContext.Current.Request;

            //判所client端是否有設定代理伺服器
            if (r.ServerVariables["HTTP_VIA"] == null)
                return r.ServerVariables["REMOTE_ADDR"].ToString();
            else
                return r.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
        }

        /// <summary>確認IP是否有回應</summary>
        public static bool PingIP(string IP)
        {
            PingReply tReply;
            IPAddress tIP = IPAddress.Parse(IP);
            using (Ping tPingControl = new Ping())
            {
                tReply = tPingControl.Send(tIP, 100);
            }

            return (tReply.Status == IPStatus.Success);
        }

        /// <summary>去除所有值的前後空白</summary>
        public static void TrimAllValue(DataTable dt)
        {
            int iCol = dt.Columns.Count;
            foreach (DataRow row in dt.Rows)
            {
                for (int i = 0; i < iCol; i++)
                {
                    row[i] = row[i].ToString().Trim();
                }
            }
        }

        /// <summary>分割字串，並在各個值前後加上指定字元</summary>
        public static string SetSplitSingleMark(string value, string SingleMark, char splitChar)
        {
            string newValue = "";
            if (value.Trim() != "")
            {
                foreach (string str in value.Split(splitChar))
                    newValue += SingleMark + str.Trim() + SingleMark + splitChar;
            }

            return newValue.Trim(splitChar);
        }

        /// <summary>DataTable轉換為JSON字串格式</summary>
        public static string DataTableConvertJson(DataTable dt)
        {
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("{\"");
            jsonBuilder.Append(dt.TableName);
            jsonBuilder.Append("\":[");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                jsonBuilder.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    jsonBuilder.Append("\"");
                    jsonBuilder.Append(dt.Columns[j].ColumnName);
                    jsonBuilder.Append("\":\"");
                    jsonBuilder.Append(dt.Rows[i][j].ToString());
                    jsonBuilder.Append("\",");
                }
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("},");
            }
            jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            jsonBuilder.Append("]");
            jsonBuilder.Append("}");
            return jsonBuilder.ToString();
        }
    }
}