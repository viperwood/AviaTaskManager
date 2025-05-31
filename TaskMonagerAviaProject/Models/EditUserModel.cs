using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskMonagerAviaProject.Models
{
    class EditUserModel
    {
        public string EmailOld { get; set; } = null!;

        public string EmailNew { get; set; } = null!;

        public string PasswordUser { get; set; } = null!;
        public string PasswordUserOld { get; set; } = null!;
        public byte[] ImageBackground { get; set; } = null!;
        public bool DarckLightColor { get; set; }

        public string Username { get; set; } = null!;

        public byte[] ImageUser { get; set; } = null!;
    }
}
