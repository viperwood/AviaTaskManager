using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskMonagerAviaProject.Models
{
    public class GetTaskModel
    {
        public int Id { get; set; }
        public string? TitleTask { get; set; }
        public string? DescriptionTask { get; set; }
        public string? TitleStatus { get; set; }
        public string? Color { get; set; }
        public List<TaskModel>? Tasks { get; set; }
        public List<UserListModel>? Users { get; set; }

    }



    public class TaskModel
    {
        public int Id { get; set; }
        public string? TitleTask { get; set; }
    }

    public class UserListModel
    {
        public int Id { get; set; }
        public string? Username { get; set; }
    }
}
