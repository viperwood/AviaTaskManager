using Avalonia.Media;
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
        public SolidColorBrush? StatusColor { get; set; }

        public List<GetUserTaskModel> userList { get; set; }
        public List<GetTasksModel> Subtask { get; set; }

        public GetTasksModel(TasksProject tasksProject)
        {
            Id = tasksProject.Id;
            TitleTask = tasksProject.TitleTask;
            Subtask = tasksProject.subtask.Select(x => new GetTasksModel(x)).ToList();
            if (tasksProject.userList != null)
            {
                userList = new List<GetUserTaskModel>();
                foreach (var task in tasksProject.userList)
                {
                    GetUserTaskModel getUserTaskModel = new GetUserTaskModel();
                    getUserTaskModel.Username = task;
                    userList!.Add(getUserTaskModel);
                }
            }
            StatusTitle = tasksProject.StatusTitle;
            StatusColor =
            new SolidColorBrush(Avalonia.Media.Color.FromArgb(
                            System.Drawing.Color.FromName(
                                tasksProject.StatusColor!).A,
                            System.Drawing.Color.FromName(
                                tasksProject.StatusColor!).R,
                            System.Drawing.Color.FromName(
                                tasksProject.StatusColor!).G,
                            System.Drawing.Color.FromName(
                                tasksProject.StatusColor!).B)
                        );
            DescriptionTask = tasksProject.DescriptionTask;

        }
    }

    class GetUserTaskModel
    {
        public string Username { get; set; }
    }
}
