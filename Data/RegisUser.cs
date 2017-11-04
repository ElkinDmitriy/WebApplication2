using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Data
{
    public class RegisUser
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]

        [RegularExpression(@"^[1-9]\d{1,12}", ErrorMessage = "Некорректный номер")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Некорректный адрес")]
        [Remote("ExistEmail", "Auth")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        public DateTime DateBirth { get; set; }

        public bool IsConfirm { get; set; }

        public string AboutMe { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [MinLength(6, ErrorMessage = "Минимальная длина пароля 6 символов")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Обязательное поле")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string PasswordConfirm { get; set; }
    }
}
