using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TaskMonagerAviaProject.Models;
using TaskMonagerAviaProject.StaticObjects;

namespace TaskMonagerAviaProject;

public partial class CreateRoleProjectWindow : Window
{
    public CreateRoleProjectWindow()
    {
        InitializeComponent();
    }

    private async void SaveRoleButtonAsync(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(NameRoleText.Text))
        {
            using (var Client = new HttpClient())
            {
                CreateRoleModel createRoleModel = new CreateRoleModel();
                createRoleModel.Email = UserAutorizationTrue.userLog.Email;
                createRoleModel.Password = UserAutorizationTrue.userLog.Password;
                createRoleModel.ProjectId = UserAutorizationTrue.ProjectId;
                createRoleModel.TitleRole = NameRoleText.Text;
                HttpResponseMessage httpResponseMessage = await Client.PostAsJsonAsync($"{BaseAddress.Address}Role/Create_role", createRoleModel);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    ErrorText.Foreground = Brushes.Green;
                    ErrorText.Text = "Роль была успешно создана!";
                    await Task.Delay(1500);
                    Close();
                }
                else
                {
                    ErrorText.Foreground = Brushes.Red;
                    ErrorText.Text = await httpResponseMessage.Content.ReadAsStringAsync();
                }
            }
        }
    }
}