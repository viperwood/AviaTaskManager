using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskMonagerAviaProject.Models
{
    public class CreateRoleModel
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public Guid ProjectId { get; set; }
        public string TitleRole { get; set; } = null!;
    }
}
