using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskMonagerAviaProject.Models
{
    class UpdateStatusTaskModel
    {
        public string? Email { get; set; }

        public string? Password { get; set; }

        public int TaskId { get; set; }
        public int StatusId { get; set; }

        public Guid ProjectId { get; set; }
    }
}
