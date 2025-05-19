using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskMonagerAviaProject.Models
{
    class GetEditSprintModel
    {
        public DateTime DateStart { get; set; }

        public DateTime DateEnd { get; set; }

        public List<TaskModel>? Tasks { get; set; }
    }
}
