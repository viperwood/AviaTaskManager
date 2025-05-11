using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TaskMonagerAviaProject.Models;
using TaskMonagerAviaProject.StaticObjects;

namespace TaskMonagerAviaProject;

public partial class CreateNewStatusTaskWindow : Window
{
    public CreateNewStatusTaskWindow()
    {
        InitializeComponent();
    }






    private async void SaveNewStatusTaskButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(NameStatusText.Text))
        {
            CreateStatusTaskModel createStatusTaskModel = new CreateStatusTaskModel();
            createStatusTaskModel.TitleStatus = NameStatusText.Text;
            createStatusTaskModel.Email = UserAutorizationTrue.userLog.Email;
            createStatusTaskModel.Password = UserAutorizationTrue.userLog.Password;
            createStatusTaskModel.ProjectId = UserAutorizationTrue.ProjectId;
            createStatusTaskModel.Color = ToHex(System.Drawing.Color.FromArgb(ColorStatus.Color.A, ColorStatus.Color.R, ColorStatus.Color.G, ColorStatus.Color.B));
            using (var Client = new HttpClient())
            {
                HttpResponseMessage httpResponseMessage = await Client.PostAsJsonAsync($"{BaseAddress.Address}StatusTask/Create_status_tasks", createStatusTaskModel);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    ErrorText.Foreground = Avalonia.Media.Brushes.Green;
                    ErrorText.Text = "Статус был успешно создан!";
                    await Task.Delay(1500);
                    Close();
                }
                else
                {
                    ErrorText.Foreground = Avalonia.Media.Brushes.Red;
                    ErrorText.Text = await httpResponseMessage.Content.ReadAsStringAsync();
                }
            }
        }
    }
    private static String ToHex(System.Drawing.Color c)
    => $"#{c.R:X2}{c.G:X2}{c.B:X2}";
}