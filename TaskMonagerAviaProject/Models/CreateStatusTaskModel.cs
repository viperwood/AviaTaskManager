using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskMonagerAviaProject.Models
{
    public class CreateStatusTaskModel
    {
        public string TitleStatus { get; set; } = null!;

        public Guid ProjectId { get; set; }

        public string Color { get; set; } = null!;

        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
