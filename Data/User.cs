using System;
using System.Collections.Generic;

using System.Linq;
using System.Web;
using System.Threading.Tasks;

namespace Data
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime DateBirth { get; set; }
        public bool IsConfirm { get; set; }
        public string AboutMe { get; set; }
        public string Password { get; set; }
    }
}
