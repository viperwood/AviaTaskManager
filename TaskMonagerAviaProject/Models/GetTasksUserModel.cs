using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskMonagerAviaProject.Models
{
    class GetTasksUserModel
    {
        public string? NameProject { get; set; }
        public List<GetTasksUserListModel>? ListTasks { get; set; }
    }

    class GetTasksUserListModel
    {
        public string? TitleTask { get; set; }
        public string? DescriptionTask { get; set; }
        public string? TitleStatus { get; set; }
        public string? Color { get; set; }
    }
}
