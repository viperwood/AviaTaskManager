using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskMonagerAviaProject.Models
{
    public class RegistrationModel
    {
        public string Email { get; set; } = null!;

        public string PasswordUser { get; set; } = null!;

        public string Username { get; set; } = null!;
    }
}
