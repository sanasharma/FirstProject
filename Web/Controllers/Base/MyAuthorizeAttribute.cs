using System;
using System.Web;
using System.Web.Mvc;

namespace EPC.Controllers.Base
{
    public class MyAuthorizeAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        public string MenuNo { set; get; }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            //判斷是否已登入
            if (Definition.UserInfo == null)
            {
                filterContext.Result = new RedirectResult("~/Home/Login");
            }
            else
            {
                //若有選單編號
                if (!string.IsNullOrEmpty(MenuNo))
                {
                    //檢查是否有頁面權限
                    bool bAuth = Tool.CheckMenuAuthority(MenuNo);
                    if (bAuth)
                    {
                        //設定目前的選單編號
                        Definition.SelectMenuNo = MenuNo;
                    }
                    else
                    {
                        filterContext.Result = new RedirectResult("~/Home/Error");
                    }
                }
                else
                {
                    //清除已選擇的選單編號
                    Definition.SelectMenuNo = "";
                }
            }
        }
    }
}