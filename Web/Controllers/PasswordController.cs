using System;
using System.Web.Mvc;
using Newtonsoft.Json;
using Models.Base;
using Models.ViewModel.Password;
using EPC.Controllers.Base;
using DataAccess;

namespace EPC.Controllers
{
    [MyAuthorize]
    public class PasswordController : Base.MyController
    {
        public ActionResult Edit()
        {
            try
            {
                //使用者登入物件
                var ui = Definition.UserInfo;

                //設定ViewModel
                var m = new PasswordEditViewModel();
                m.ID = ui.ID;
                m.Account = ui.Account;
                m.Email = ui.Email;

                return View(m);
            }
            catch
            {
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public JsonResult Edit(PasswordEditViewModel m)
        {
            Result r = new Result();

            try
            {
                //修改密碼
                DataAccess.PasswordDataAccess.EditPassword(m);
                r.Set(ResultCode.Success, "作業成功，請用新密碼重新登入");
            }
            catch (Exception e)
            {
                r.Set(ResultCode.Error, e.Message);
            }

            return Json(r, JsonRequestBehavior.AllowGet);
        }
    }
}