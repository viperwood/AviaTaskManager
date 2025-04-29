using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using TaskMonagerAviaProject.Models;
using TaskMonagerAviaProject.StaticObjects;

namespace TaskMonagerAviaProject;

public partial class CreateProjectWindow : Window
{
    public CreateProjectWindow()
    {
        InitializeComponent();
    }

    private void SaveButtonAsync(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        SaveNewProject();
    }

    private async void SaveNewProject()
    {
        if (!string.IsNullOrEmpty(NameProjectText.Text))
        {
            CreateProjectModel createProjectModel = new CreateProjectModel();
            createProjectModel.NameProject = NameProjectText.Text;
            createProjectModel.Email = UserAutorizationTrue.userLog.Email;
            createProjectModel.Password = UserAutorizationTrue.userLog.Password;
            using (var Client = new HttpClient())
            {
                HttpResponseMessage httpResponseMessage = await Client.PostAsJsonAsync($"{BaseAddress.Address}Project/Creating_project", createProjectModel);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    ErrorText.Foreground = Brushes.Green;
                    ErrorText.Text = "Проект был успешно создан!";
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
        else
        {
            ErrorText.Foreground = Brushes.Red;
            ErrorText.Text = "Все поля должны быть заполнены!";
        }
    }
}