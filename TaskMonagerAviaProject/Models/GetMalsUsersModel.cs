using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskMonagerAviaProject.Models
{
    public class GetMalsUsersModel
    {
        public int Id { get; set; }
        public int AddresseeId { get; set; }
        public int DestinationId { get; set; }
        public int? AnswerMailId { get; set; }
        public string? AnswerMailText { get; set; }
        public string? TitleStatus { get; set; }
        public string? TextMail { get; set; }
        public DateTime DateAndTimeMail { get; set; }
    }
}
