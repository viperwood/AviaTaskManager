using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TaskMonagerAviaProject.Models;
using TaskMonagerAviaProject.StaticObjects;

namespace TaskMonagerAviaProject;

public partial class AddUserProjectWindow : Window
{
    public AddUserProjectWindow()
    {
        InitializeComponent();
        LoadInfo();
    }

    private List<GetRoleModel> getRoleModel = new List<GetRoleModel>();

    private async void LoadInfo()
    {
        using (var Client = new HttpClient())
        {
            HttpResponseMessage httpResponseMessage = await Client.GetAsync($"{BaseAddress.Address}Role/Get_role?Email={UserAutorizationTrue.userLog.Email}&Password={UserAutorizationTrue.userLog.Password}&ProjectId={UserAutorizationTrue.ProjectId}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string context = await httpResponseMessage.Content.ReadAsStringAsync();
                getRoleModel = JsonConvert.DeserializeObject<List<GetRoleModel>>(context)!.ToList();
                RolesComboBox.ItemsSource = getRoleModel.Where(x => x.Id != 1).ToList();
            }
        }
    }

    private async void AddUserButtonAsync(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (RolesComboBox.SelectedIndex != -1 && !string.IsNullOrEmpty(NameUseerText.Text))
        {
            using (var Client = new HttpClient())
            {
                int id = getRoleModel[RolesComboBox.SelectedIndex + 1].Id;
                CreateObjectModel createObjectModel = new CreateObjectModel();
                createObjectModel.Email = UserAutorizationTrue.userLog.Email;
                createObjectModel.Password = UserAutorizationTrue.userLog.Password;
                createObjectModel.ProjectId = UserAutorizationTrue.ProjectId;
                createObjectModel.User = NameUseerText.Text;
                createObjectModel.roleId = id;
                HttpResponseMessage httpResponseMessage = await Client.PostAsJsonAsync($"{BaseAddress.Address}Project/Add_object", createObjectModel);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    ErrorText.Foreground = Brushes.Green;
                    ErrorText.Text = "Предложение успешно отправленно!";
                    await Task.Delay(1500);
                    ErrorText.Text = "";
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