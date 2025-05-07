using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskMonagerAviaProject.Models
{
    public class GetUsersProjectModel
    {
        public string TitleRole { get; set; } = null!;

        public string Username { get; set; } = null!;

        public int? UserId { get; set; }
    }
}
