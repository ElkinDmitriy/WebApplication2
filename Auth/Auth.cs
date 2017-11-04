using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Security.Cryptography;
using System.Net.Mail;
using System.Web.Configuration;

namespace Utils.Auth
{
    public static class Auth
    {
        public static bool IsOut(HttpContextBase MyHttpContext, Implementation.UsersRepository RepUser)
        {
            bool result = true;
            try
            {
                string id = MyHttpContext.Request.Cookies["Test Site"].Values["id"];
                string password = MyHttpContext.Request.Cookies["Test Site"].Values["password"];

                if (id == "" || password == "")
                {
                    return true;
                }

                try
                {
                    Data.User tmpUser = RepUser.GetAll().Single(u => u.Id.ToString() == id && u.Password == password);
                    if (tmpUser != null) { result = false; }
                }
                catch (Exception ex) { return true; }
                
            }
            catch (Exception ex)
            {

            }

            return result;
        }


        public static string GetHash(string source)
        {
            string result = "";
            using (MD5 MyMD5 = MD5.Create())
            {
                byte[] data = MyMD5.ComputeHash(Encoding.UTF8.GetBytes(source));

                StringBuilder SBilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    SBilder.Append(data[i].ToString());
                }

                result = SBilder.ToString();
            }

            return result;
        }

        public static bool ExecuteIn(Data.LoginUser logUser, Implementation.UsersRepository RepUsers, HttpContextBase MyHttpContext)
        {
            try
            {
                
                    string HPas = GetHash(logUser.Password);
                    try
                    {
                        Data.User tmpUser = RepUsers.GetAll().Single(u => u.Email == logUser.Email && u.Password == HPas && u.IsConfirm == true);

                        HttpCookie cookie = new HttpCookie("Test Site");

                        cookie["id"] = tmpUser.Id.ToString();
                        cookie["password"] = HPas;

                        MyHttpContext.Response.Cookies.Add(cookie);

                       
                        return true;

                    }
                    catch (Exception ex)
                    {
                        
                        return false;
                    }
                
            }
            catch (Exception ex)
            {
                //MyHttpContext.Response.Redirect("/Test/auth/?er=1");
                return false;
            }
        }

        private static bool SendMail(Data.LoginUser logUser)
        {
            SmtpClient Smtp = new SmtpClient("smtp.yandex.ru", 25);

            string Address = WebConfigurationManager.AppSettings.GetValues("Address")[0];
            string Password = WebConfigurationManager.AppSettings.GetValues("Password")[0];

            Smtp.EnableSsl = true;
            Smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            Smtp.UseDefaultCredentials = false;


            Smtp.Credentials = new NetworkCredential(Address, Password);
            MailMessage Message = new MailMessage();
            Message.From = new MailAddress(Address);
            Message.To.Add(new MailAddress(logUser.Email));
            Message.Subject = WebConfigurationManager.AppSettings.GetValues("SubjectMail")[0];
            Message.Body = WebConfigurationManager.AppSettings.GetValues("BodyMail")[0] + logUser.Password;

            try
            {
                Smtp.Send(Message);
            }
            catch (SmtpException ex)
            {
                return false;
            }

            return true;
        }


        public static void ExecuteOut(HttpContextBase MyHttpContext)
        {
            try
            {
                MyHttpContext.Response.Cookies["Test Site"].Values.Clear();
            }
            catch (Exception) { }


        }


        public static bool IsExist(string email, Implementation.UsersRepository RepUsers)
        {
            
                if (RepUsers.GetAll().Count() == 0) { return false; }
                try
                {
                    Data.User tmpUser = RepUsers.GetAll().Single(u => u.Email == email);
                }
                catch (Exception ex) { return false; }

                return true;
            
        }


        public static void Reg(Data.RegisUser regUser, Implementation.UsersRepository RepUsers)
        {
           
            regUser.Password = GetHash(regUser.Password);
            Data.User newUser = new Data.User();
            newUser.FirstName = regUser.FirstName;
            newUser.LastName = regUser.LastName;
            newUser.Email = regUser.Email;
            newUser.PhoneNumber = regUser.PhoneNumber;
            newUser.Password = regUser.Password;
            newUser.IsConfirm = false;

            RepUsers.Create(newUser);
                try
                {
                    RepUsers.SaveChange();
                }
                catch (Exception ex)
                {
                    ;
                }
            

            if (SendMail(new Data.LoginUser { Email = regUser.Email, Password = regUser.Password}))
            {

            }
            else
            {

            }
        }


        private static bool SendNewPass(Data.LoginUser logUser)
        {
            SmtpClient Smtp = new SmtpClient("smtp.yandex.ru", 25);

            string Address = WebConfigurationManager.AppSettings.GetValues("Address")[0];
            string Password = WebConfigurationManager.AppSettings.GetValues("Password")[0];

            Smtp.EnableSsl = true;
            Smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            Smtp.UseDefaultCredentials = false;


            Smtp.Credentials = new NetworkCredential(Address, Password);
            MailMessage Message = new MailMessage();
            Message.From = new MailAddress(Address);
            Message.To.Add(new MailAddress(logUser.Email));
            Message.Subject = "Новый пароль";
            Message.Body = "Добрый день! вы воспользовались сервисом востановления пароля. Ваш новый пароль: " + logUser.Password;

            try
            {
                Smtp.Send(Message);
            }
            catch (SmtpException ex)
            {
                return false;
            }

            return true;
        }



        public static void GetNewPass(string email, Implementation.UsersRepository RepUsers, HttpContextBase MyHttpContext)
        {
            char[] data = { 'a', 'b', 'c', 'd', 'e', 'f', 'g',
                'A', 'B', 'C', 'D', 'E', 'F', 'G',
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            Random rnd = new Random();
            StringBuilder SB = new StringBuilder();
            for (int i = 0; i < 6; i++)
            {
                int index = rnd.Next(data.Count() - 1);
                SB.Append(data[index]);
            }

            string NewPass = SB.ToString(); //пароль
            string HPass = GetHash(NewPass); // хеш пароля

            
                try
                {
                    Data.User tmpUser = RepUsers.GetAll().Single(u => u.Email == email);
                    tmpUser.Password = HPass;
                    
                    if (SendNewPass(new Data.LoginUser { Email = email, Password = NewPass}))// пытаемся отослать сообщение с новым паролем
                    {
                        RepUsers.SaveChange();

                        HttpCookie cookie = new HttpCookie("Test Site");

                        cookie["email"] = email;

                        MyHttpContext.Response.Cookies.Add(cookie);
                    }


                }
                catch (Exception ex) { }
            

        }


        public static void Confirm(string id, Implementation.UsersRepository RepUsers)
        {
            
                var Users = RepUsers.GetAll().Where(u => u.IsConfirm == false && u.Password == id);

                if (Users != null)
                {
                    foreach (Data.User item in Users)// всех пользователей с этим хешем подтверждаем o_O ))))
                    {
                        item.IsConfirm = true;
                        
                    }

                    RepUsers.SaveChange();
                }

            
        }
    }
}
