using System;
using System.Data;
using System.Web.Mvc;
using System.Collections.Generic;
using Newtonsoft.Json;
using Models.Base;
using Models.Model;
using Models.ViewModel;
using DataAccess;
using Tool;

namespace EPC.Controllers
{
    public class HomeController : Controller
    {
        [Base.MyAuthorize(MenuNo = "00")]
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            return View();
        }

        public ActionResult Login(string id, string type)
        {
            //登出
            if (type == "logout")
            {
                Session.Clear();
            }
            //模擬帳號
            else if (type == "Simulate")
            {
                Session.Clear();
                TempData["Simulate"] = id;

                //建立登入模型
                LoginViewModel v = new LoginViewModel();
                v.Account = id;
                v.Password = "000";

                //呼叫登入函式
                Login(v);
            }

            //判斷有無Session登入物件
            if (Definition.UserInfo != null)
            {
                return RedirectToAction("Index", "Home");
            }

            LoginViewModel m = new LoginViewModel();
            return View(m);
        }

        [HttpPost]
        public JsonResult Login(LoginViewModel m)
        {
            var r = new Result();

            //基本驗証
            if (string.IsNullOrEmpty(m.Account) || string.IsNullOrEmpty(m.Password))
            {
                r.Code = ResultCode.Error;
                r.Msg = "帳號密碼請勿空白";
                return Json(r, JsonRequestBehavior.AllowGet);
            }

            //判斷是否為模擬使用者，並取得使用者資訊
            var pwd = (TempData["Simulate"] == null ? Util.MD5(m.Password, 32) : null);
            var dt = UserDataAccess.GetUserList(null, m.Account, pwd, null, null, new Pages());

            if (dt.Rows.Count > 0)
            {
                //判斷帳戶是否鎖定
                DataRow rowUser = dt.Rows[0];
                if (!Convert.ToBoolean(rowUser["IsLock"]))
                {
                    //將使用者資訊記錄Session
                    var u = Util.ToList<User>(dt)[0];
                    Definition.UserInfo = u;

                    //將使用者頁面權限記錄Session
                    var dtAuth = UserDataAccess.GetUserAuthority(u.Account);
                    Definition.UserAuthority = Util.ToList<Auth>(dtAuth);

                    #region 個人化選單資料
                    var menuList = new List<Menu>();

                    //取得全部的選單
                    var dtMenu = MenuDataAccess.GetAllMenuList(true);
                    var listMenu = Util.ToList<Menu>(dtMenu);

                    foreach (var menu in listMenu) //逐筆讀取選單資料
                    {
                        if (Tool.CheckMenuAuthority(menu.MenuNo)) //驗証權限
                        {
                            menuList.Add(menu); //增加選單
                        }
                    }

                    Definition.MenuList = menuList;
                    #endregion

                    //取得登錄者電腦IP
                    m.IP = Util.GetClientIP();

                    //更新使用者登入時間
                    UserDataAccess.SetUserLoginDate(m);
                    r.Set(ResultCode.Success, "登入成功");
                }
                else
                {
                    r.Set(ResultCode.Error, "帳戶已鎖定");
                }
            }
            else
            {
                r.Set(ResultCode.Error, "登入失敗");
            }

            return Json(r, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Error()
        {
            return View();
        }
    }
}