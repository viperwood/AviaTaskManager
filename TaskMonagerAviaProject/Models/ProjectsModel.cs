using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskMonagerAviaProject.Models
{
    class ProjectsModel
    {
        public Guid ProjectId { get; set; }
        public string? TitleRole { get; set; }
        public string? NameProject { get; set; }
    }
}
