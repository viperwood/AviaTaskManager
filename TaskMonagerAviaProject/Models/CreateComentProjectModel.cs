using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskMonagerAviaProject.Models
{
    public class CreateComentProjectModel
    {
        public string TextMail { get; set; } = null!;

        public int? AnswerId { get; set; }

        public Guid ProjectId { get; set; }

        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
