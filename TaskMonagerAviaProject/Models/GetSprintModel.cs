using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskMonagerAviaProject.Models
{
    public class GetSprintModel
    {
        public int Id { get; set; }
        public string? TitleStatus { get; set; }
        public string? Color { get; set; }
        public List<GetSprintTasksModel>? Tasks { get; set; }

    }

    public class GetSprintTasksModel
    {
        public int Id { get; set; }
        public string? TitleTask { get; set; }

    }
}
