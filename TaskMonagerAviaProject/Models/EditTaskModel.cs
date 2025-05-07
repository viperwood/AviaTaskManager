using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskMonagerAviaProject.Models
{
    public class EditTaskModel
    {
        public Guid ProjectId { get; set; }

        public string TitleTask { get; set; } = null!;

        public string DescriptionTask { get; set; } = null!;

        public int? TaskId { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        public List<int>? ExecutorId { get; set; }

        public List<int>? TasksId { get; set; }
    }
}
