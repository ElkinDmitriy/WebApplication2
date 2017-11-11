using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace WebApplication2.Controllers
{
    public class AuthController : Controller
    {

        Implementation.UsersRepository RepUsers;

        public AuthController()
        {
            this.RepUsers = new Implementation.UsersRepository();
        }

        // GET: Auth
        public ActionResult Index()
        {
            return Redirect("Test/index");
        }

        //===========================================================
        [HttpGet]
        public ActionResult Login(int? er)
        {
            if (!Utils.Auth.Auth.IsOut(this.HttpContext, this.RepUsers)) { return Redirect("/Test/index"); }

            if (er != null) { ViewBag.Er = "Не правильные данные для входа"; } else { ViewBag.Er = ""; }
            return View();
        }

        [HttpPost]
        public ActionResult Login(Data.LoginUser loginUser)
        {

            if (!Utils.Auth.Auth.IsOut(this.HttpContext, this.RepUsers)) { return Redirect("/Test/index"); }

            if(Utils.Auth.Auth.ExecuteIn(loginUser, this.RepUsers, this.HttpContext))
            {
                return Redirect("/Test/Index");
            }
            else
            {
                return Redirect("/Auth/login/?er=1");
            }

            return View();
        }
        //===========================================================

        [HttpGet]
        public ActionResult Logout()
        {
            Utils.Auth.Auth.ExecuteOut(this.HttpContext);
            return Redirect("/Test/index");
        }

        //===========================================================

        [HttpGet]
        public ActionResult Regis()
        {
            if (!Utils.Auth.Auth.IsOut(this.HttpContext, this.RepUsers)) { return Redirect("/Test/index"); }

            return View();
            
        }

        [HttpPost]
        public ActionResult Regis(Data.RegisUser regisUser)
        {
            if (!Utils.Auth.Auth.IsOut(this.HttpContext, this.RepUsers)) { return Redirect("/Test/index"); }

            if (!Utils.Auth.Auth.IsExist(regisUser.Email, this.RepUsers)) // Если пользователя с такой почтой нет
            {
                
                

                //устанавливаем временные куки для информативной страницы
                HttpCookie cookie = new HttpCookie("Test Site");

                cookie["email"] = regisUser.Email;

                HttpContext.Response.Cookies.Add(cookie);

                try
                {
                    Utils.Auth.Auth.Reg(regisUser, this.RepUsers);
                    
                }
                catch (Exception ex)
                {
                    return Redirect("/Auth/inforegis/?id=false");
                }

                return Redirect("/Auth/inforegis/?id=true");
            }


            return Redirect("/Auth/inforegis/?id=true"); //НАДО ОБДУМАТь результат проверки на существующий email пока такой вариант
        }

        //===========================================================

        [HttpGet]
        public ActionResult Confirm(string id)
        {
            Utils.Auth.Auth.Confirm(id, this.RepUsers);

            return Redirect("/Test/Index");
        }


        //===========================================================

        [HttpGet]
        public ActionResult InfoRegis(string id)
        {
            if (HttpContext.Request.Cookies.Count == 0) { return Redirect("/Test/index"); }

            string tmpEmail = HttpContext.Request.Cookies["Test Site"].Values["email"];
            if (tmpEmail == "")
            {
                return Redirect("/Test/index");
            }
            else
            {
                HttpContext.Response.Cookies["Test Site"].Values.Set("email", "");
            }

            ViewBag.IsRegis = id;
            return View();
        }

        //===========================================================

        [HttpGet]
        public ActionResult InfoGetPass(string id)
        {
            if (HttpContext.Request.Cookies.Count == 0) { return Redirect("/Test/index"); }

            string tmpEmail = HttpContext.Request.Cookies["Test Site"].Values["email"];
            if (tmpEmail == "")
            {
                return Redirect("/Test/index");
            }
            else
            {
                HttpContext.Response.Cookies["Test Site"].Values.Set("email", "");
            }


            ViewBag.IsGetPass = id;
            return View();
        }

        //===========================================================

        [HttpGet]
        public ActionResult GetPass()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetPass(Data.GetPassUser getPassUser)
        {
            try
            {
                Utils.Auth.Auth.GetNewPass(getPassUser.Email, this.RepUsers, this.HttpContext);
                return Redirect("/Auth/infogetpass/?id=true");
            }
            catch (Exception ex) { return Redirect("/Auth/infogetpass/?id=false"); }
        }

        //============================================================

        public JsonResult ExistEmail(string Email)
        {
            
                try
                {
                    Data.User tmpUser = RepUsers.GetAllWhere(u => u.Email == Email).First();
                }
                catch (Exception)
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }

                return Json("Адрес занят", JsonRequestBehavior.AllowGet);

            
        }

        public JsonResult ValidateEmail(string Email)
        {
            
                try
                {
                    Data.User tmpUser = RepUsers.GetAllWhere(u => u.Email == Email).First();
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {
                    return Json("Указанная почта не зарегистрирована", JsonRequestBehavior.AllowGet);
                }

            
        }

        public JsonResult TrySetNewEmail(string Email)
        {

            int id = int.Parse(HttpContext.Request.Cookies["Test Site"].Values["id"]);
            try
            {
                Data.User tmpUser = RepUsers.GetForId(id);
                if(tmpUser != null)
                {
                    if(tmpUser.Email != Email)
                    {
                        RepUsers.GetAllWhere(u => u.Email == Email).First();
                        return Json("Этот адрес занят", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(true, JsonRequestBehavior.AllowGet);
                    }
                }

            }
            catch (Exception ex) { }

            return Json(true, JsonRequestBehavior.AllowGet);
        }


    }
}