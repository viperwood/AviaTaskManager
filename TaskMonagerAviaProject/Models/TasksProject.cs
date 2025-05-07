using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskMonagerAviaProject.Models
{
    public partial class TasksProject
    {
        public int Id { get; set; }

        public Guid ProjectId { get; set; }

        public string TitleTask { get; set; } = null!;
        public string StatusTitle { get; set; } = null!;
        public string StatusColor { get; set; } = null!;

        public List<string> userList { get; set; }

        public List<TasksProject> subtask { get; set; }

        public string DescriptionTask { get; set; } = null!;

        public int StatusId { get; set; }
    }
}
