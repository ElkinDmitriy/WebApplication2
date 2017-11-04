using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class EditUserGeneral
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [RegularExpression(@"^[1-9]\d{1,12}", ErrorMessage = "Некорректный номер")]
        public string PhoneNumber { get; set; }
        public DateTime DateBirth { get; set; }
        public string AboutMe { get; set; }

    }
}
