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
using System.ComponentModel;

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
                    Data.User tmpUser = RepUser.GetAllWhere(u => u.Id.ToString() == id && u.Password == password).First();
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
                        
                    Data.User tmpUser = RepUsers.GetAllWhere(u => u.Email == logUser.Email && u.Password == HPas && u.IsConfirm == true).First();

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




        private static void SendMail(Data.LoginUser logUser, string subject, string body)
        {
            SmtpClient Smtp = new SmtpClient("smtp.yandex.ru", 587); //587

            string Address = WebConfigurationManager.AppSettings.GetValues("Address")[0];
            string Password = WebConfigurationManager.AppSettings.GetValues("Password")[0];

            Smtp.EnableSsl = true;
            Smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            Smtp.UseDefaultCredentials = false;
            


            Smtp.Credentials = new NetworkCredential(Address, Password);
            MailMessage Message = new MailMessage();
            Message.From = new MailAddress(Address);
            Message.To.Add(new MailAddress(logUser.Email));
            Message.Subject = subject;
            Message.Body = body;
            



            try
            {
                Smtp.Send(Message);
                
            }
            catch (SmtpException ex)
            {
                throw new Exception(ex.Message, ex);
            }

            
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

            IEnumerable<Data.User> ListUsers = RepUsers.GetAllWhere();
                if (ListUsers.Count() == 0) { return false; }
                try
                {
                    Data.User tmpUser = ListUsers.Single(u => u.Email == email);
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

            
            try
            {
                SendMail(new Data.LoginUser { Email = regUser.Email, Password = regUser.Password },
                    WebConfigurationManager.AppSettings.GetValues("SubjectMailConfirm")[0],
                    WebConfigurationManager.AppSettings.GetValues("BodyMailConfirm")[0] + regUser.Password);

                RepUsers.Create(newUser);
                RepUsers.SaveChange();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

            
            
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

            HttpCookie cookie = new HttpCookie("Test Site");

            cookie["email"] = email;

            MyHttpContext.Response.Cookies.Add(cookie);


            try
                {
                    

                    SendMail(new Data.LoginUser { Email = email, Password = NewPass },
                                    WebConfigurationManager.AppSettings.GetValues("SubjectMailNewPass")[0],
                                    WebConfigurationManager.AppSettings.GetValues("BodyMailNewPass")[0] + NewPass);


                    Data.User tmpUser = RepUsers.GetAllWhere(u => u.Email == email).First();
                    tmpUser.Password = HPass;
                    RepUsers.SaveChange();

              
                }
                catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            

        }


        public static void Confirm(string id, Implementation.UsersRepository RepUsers)
        {
            
                var Users = RepUsers.GetAllWhere(u => u.IsConfirm == false && u.Password == id);

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
