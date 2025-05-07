using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using TaskMonagerAviaProject.Models;
using TaskMonagerAviaProject.StaticObjects;

namespace TaskMonagerAviaProject;

public partial class CreateNewStatusTaskWindow : Window
{
    public CreateNewStatusTaskWindow()
    {
        InitializeComponent();
    }

    private void SaveNewStatusTaskButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(NameStatusText.Text))
        {
            CreateStatusTaskModel createStatusTaskModel = new CreateStatusTaskModel();
            createStatusTaskModel.TitleStatus = NameStatusText.Text;
            createStatusTaskModel.Email = UserAutorizationTrue.userLog.Email;
            createStatusTaskModel.Password = UserAutorizationTrue.userLog.Password;
            createStatusTaskModel.ProjectId = UserAutorizationTrue.ProjectId;
            var a = ColorStatus.HsvColor;

        }
    }
}