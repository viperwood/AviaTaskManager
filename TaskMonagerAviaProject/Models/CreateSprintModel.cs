using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskMonagerAviaProject.Models
{
    class CreateSprintModel
    {
        public Guid ProjectId { get; set; }

        public DateTime DateStart { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }

        public DateTime DateEnd { get; set; }
        public int SprintId { get; set; }

        public List<int>? IdTasks { get; set; }
    }
}
