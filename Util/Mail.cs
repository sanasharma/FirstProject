using System.IO;
using System.Web;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Configuration;

namespace Tool
{
    public class TemplateMail
    {
        private string CONTENT = "";

        public TemplateMail(string TemplatePath)
        {
            //讀檔內容
            string _path = HttpContext.Current.Server.MapPath(TemplatePath);
            StreamReader strReader = new StreamReader(_path);
            CONTENT = strReader.ReadToEnd();
            strReader.Close();
            strReader.Dispose();
        }

        //取代項目值
        public string this[string Item]
        {
            set { CONTENT = CONTENT.Replace(Item, value); }
        }



        //覆寫ToString函式
        public override string ToString()
        {
            return CONTENT;
        }
    }

    public class Mail : MailMessage
    {
        public Mail()
        {

        }

        public void SendMail()
        {
            string strMailServer = ConfigurationManager.ConnectionStrings["MailServer"].ConnectionString;
            string strAccount = ConfigurationManager.ConnectionStrings["MailAccount"].ConnectionString;
            string strPassword = ConfigurationManager.ConnectionStrings["MailPassword"].ConnectionString;

            SmtpClient smtpClient = new SmtpClient(strMailServer);
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(strAccount, strPassword);
            smtpClient.Send(this);
        }
    }
}