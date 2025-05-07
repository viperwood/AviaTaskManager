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

    public CreateProjectWindow(Guid IdProject)
    {
        InitializeComponent();
        LoadProject();
        EditProject.IsVisible = true;
        CreateNewProject.IsVisible = false;
    }


    private async void LoadProject()
    {
        using (var Client = new HttpClient())
        {
            HttpResponseMessage httpResponseMessage = await Client.GetAsync($"{BaseAddress.Address}Project/Get_project?Email={UserAutorizationTrue.userLog.Email}&Password={UserAutorizationTrue.userLog.Password}&ProjectId={UserAutorizationTrue.ProjectId}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                List<ProjectsModel> projects = JsonConvert.DeserializeObject<List<ProjectsModel>>(await httpResponseMessage.Content.ReadAsStringAsync())!;
                NewNameProjectText.Text = projects.Select(x => x.NameProject).ToList()[0];
            }
        }
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

    private async void SaveEditButtonAsync(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(NewNameProjectText.Text))
        {
            EditProjectModel editProjectModel = new EditProjectModel();
            editProjectModel.NameProject = NewNameProjectText.Text;
            editProjectModel.Email = UserAutorizationTrue.userLog.Email;
            editProjectModel.Password = UserAutorizationTrue.userLog.Password;
            editProjectModel.ProjectId = UserAutorizationTrue.ProjectId;
            using (var Client = new HttpClient())
            {
                HttpResponseMessage httpResponseMessage = await Client.PutAsJsonAsync($"{BaseAddress.Address}Project/Editing_project", editProjectModel);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    ErrorTextEdit.Foreground = Brushes.Green;
                    ErrorTextEdit.Text = "Проект был изменён!";
                    await Task.Delay(1500);
                    Close();
                }
                else
                {
                    ErrorTextEdit.Foreground = Brushes.Red;
                    ErrorTextEdit.Text = await httpResponseMessage.Content.ReadAsStringAsync();
                }
            }
        }
        else
        {
            ErrorTextEdit.Foreground = Brushes.Red;
            ErrorTextEdit.Text = "Все поля должны быть заполнены!";
        }
    }

    private async void DeleteProjectButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        using (var Client = new HttpClient())
        {
            HttpResponseMessage httpResponseMessage = await Client.DeleteAsync($"{BaseAddress.Address}Project/Delete_project?Email={UserAutorizationTrue.userLog.Email}&Password={UserAutorizationTrue.userLog.Password}&ProjectId={UserAutorizationTrue.ProjectId}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                ErrorTextEdit.Foreground = Brushes.Green;
                ErrorTextEdit.Text = "Проект удален!";
                UserAutorizationTrue.ProjectId = Guid.Empty;
                await Task.Delay(1500);
                Close();
            }
            else
            {
                ErrorTextEdit.Foreground = Brushes.Red;
                ErrorTextEdit.Text = await httpResponseMessage.Content.ReadAsStringAsync();
            }
        }
    }
}