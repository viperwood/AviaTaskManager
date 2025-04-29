using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskMonagerAviaProject.Models
{
    class GetTasksModel
    {
        public int Id { get; set; }
        public string? TitleTask { get; set; }
        public string? DescriptionTask { get; set; }
        public string? StatusTitle { get; set; }
        public string? StatusColor { get; set; }
    }
}
