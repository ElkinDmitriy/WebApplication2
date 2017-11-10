using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utils.Auth;

namespace WebApplication2.Controllers
{
    public class TestController : Controller
    {

        
        DefinitionRepositories.IRepositiry<Data.User> RepUsers;

        public TestController()
        {
            RepUsers = new Implementation.UsersRepository();
        }



        public ActionResult Index()
        {

            if (Utils.Auth.Auth.IsOut(this.HttpContext, this.RepUsers as Implementation.UsersRepository))
            {
                return Redirect("/Auth/login");
            }
            else
            {
                
                return Redirect("/Test/pageuser/?id=" + HttpContext.Request.Cookies["Test Site"].Values["id"]);
            }
            
        }
       

        [HttpGet]
        public ActionResult PageUser(int? id)
        {

            if (Utils.Auth.Auth.IsOut(this.HttpContext, this.RepUsers as Implementation.UsersRepository))
            {
                return Redirect("/Auth/login");
            }

            if (id == null)
            {
                return Redirect("/Test/index");
            }
            else
            {
               
                    try
                    {
                    int idFromCookie = int.Parse(HttpContext.Request.Cookies["Test Site"].Values["id"]);
                        if (id == idFromCookie)
                    {
                        Data.User UserCurrent = RepUsers.GetForId(idFromCookie);
                        ViewBag.User = UserCurrent;
                    }
                    
                        
                    }
                    catch (Exception ex) { return Redirect("/Test/index"); }
                
            }
            return View();
        }



        [HttpGet]
        public ActionResult EditUser()
        {
            

            if (Utils.Auth.Auth.IsOut(this.HttpContext, RepUsers as Implementation.UsersRepository))
            {
                return Redirect("/Auth/login");
            }
            else
            {
                
                    try
                    {
                        int id = int.Parse(HttpContext.Request.Cookies["Test Site"].Values["Id"]);
                        Data.User tmpUser = RepUsers.GetAllWhere(u => u.Id == id).First();
                        ViewBag.OldInfoUser = tmpUser;
                    }catch(Exception ex)
                    {

                    }
                

                    return View();
            }
        }


        [HttpPost]
        public ActionResult General_Info(Data.EditUserGeneral gUsr)
        {

            if (gUsr.DateBirth != null)
            {
                int id = int.Parse(HttpContext.Request.Cookies["Test Site"].Values["id"]);

                Data.User tmpUser = RepUsers.GetForId(id);

                if (gUsr.FirstName != "" && gUsr.FirstName != null) { tmpUser.FirstName = gUsr.FirstName; }
                tmpUser.DateBirth = gUsr.DateBirth;
                if (gUsr.LastName != "" && gUsr.LastName != null) { tmpUser.LastName = gUsr.LastName; }
                if (gUsr.PhoneNumber != "" && gUsr.PhoneNumber != null) { tmpUser.PhoneNumber = gUsr.PhoneNumber; }
                if (gUsr.AboutMe != "") { tmpUser.AboutMe = gUsr.AboutMe; }

                RepUsers.Update(tmpUser);
                RepUsers.SaveChange();
            }

            
            return Redirect("/Test/edituser");
        }

        [HttpPost]
        public ActionResult Auth_Info(Data.EditUserAuth authUsr)
        {
            
            int id = int.Parse(HttpContext.Request.Cookies["Test Site"].Values["id"]);

            Data.User tmpUser = RepUsers.GetForId(id);

            if (authUsr.Email != "" && authUsr.Email != null) { tmpUser.Email = authUsr.Email; }
            if (authUsr.Password != "" && authUsr.Password != null) { tmpUser.Password = Utils.Auth.Auth.GetHash(authUsr.Password); }

            RepUsers.Update(tmpUser);
            RepUsers.SaveChange();

            

            return Redirect("/Test/edituser");
        }

    }
}