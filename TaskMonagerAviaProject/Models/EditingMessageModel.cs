using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskMonagerAviaProject.Models
{
    public class EditingMessageModel
    {
        public string TextMail { get; set; } = null!;

        public string? Email { get; set; }
        public string? Password { get; set; }

        public int? MessageId { get; set; }
    }
}
