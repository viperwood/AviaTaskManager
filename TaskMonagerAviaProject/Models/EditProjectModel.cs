using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskMonagerAviaProject.Models
{
    class EditProjectModel
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
        public Guid ProjectId { get; set; }
        public string NameProject { get; set; } = null!;
    }
}
