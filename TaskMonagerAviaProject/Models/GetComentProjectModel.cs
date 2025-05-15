using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskMonagerAviaProject.Models
{
    class GetComentProjectModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int AnswerMailId { get; set; }
        public string? AnswerMailText { get; set; }
        public string? AnswerMailUser { get; set; }
        public string? Username { get; set; }
        public string? TitleStatus { get; set; }
        public string? TextMail { get; set; }
        public DateTime DateAndTimeComent { get; set; }
        public byte[]? ImageMailProject { get; set; }
    }
}
