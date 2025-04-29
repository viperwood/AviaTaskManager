using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;

using Avalonia.Media;
using System.IO;
using System.Linq;
using System.Net.Http;
using TaskMonagerAviaProject.Models;
using TaskMonagerAviaProject.StaticObjects;
using System.Threading.Tasks;

namespace TaskMonagerAviaProject;

public partial class WindowProjects : Window
{
    public WindowProjects()
    {
        InitializeComponent();
        _content = File.ReadAllText(path);
        ImageTest.Source = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + @"\Images\1667650479147654979.jpg");
    }

    private string path = AppDomain.CurrentDomain.BaseDirectory + @"\UserLog.json";
    private string _content = "";
    
    private bool _isPanelOpen = false;
    private bool _isPanelProjectOpen = true;

    public WindowProjects(string content)
    {
        InitializeComponent();
        if (!File.Exists(path))
        {
            SystemMessage.IsVisible = true;
            _content = content;
        }
        UserName.Text = UserAutorizationTrue.userLog.Username;
        ImageTest.Source = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + @"\Images\1667650479147654979.jpg");
        LoadProjects();
    }

    private void YesSaveButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(_content) && !string.IsNullOrEmpty(path))
        {
            File.WriteAllText(path, _content);
            SystemMessage.IsVisible = false;
        }
    }

    private void NoSaveButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        SystemMessage.IsVisible = false;
    }

    private void ExitAccaunt(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        UserAutorizationTrue.userLog = new User();
        MainWindow mainWindow = new MainWindow();
        mainWindow.Show();
        Close();
    }

    private void ButtonPanel(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _isPanelOpen = !_isPanelOpen;
        Panel.IsPaneOpen = _isPanelOpen;
    }
    private void ButtonPanelProject(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        _isPanelProjectOpen = !_isPanelProjectOpen;
        PanelProjects.IsPaneOpen = _isPanelProjectOpen;
        ButtonOpenMainPanel.IsVisible = _isPanelProjectOpen;
        ButtonCloseMainPanel.IsVisible = !_isPanelProjectOpen;
        if (_isPanelProjectOpen)
        {
            LoadProjects();
        }
    }

    private async void LoadProjects()
    {
        using (var Client = new HttpClient())
        {
            HttpResponseMessage httpResponseMessage = await Client.GetAsync($"{BaseAddress.Address}Project/Get_projects?Email={UserAutorizationTrue.userLog.Email}&Password={UserAutorizationTrue.userLog.Password}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                List<ProjectsModel> projects = JsonConvert.DeserializeObject<List<ProjectsModel>>(await httpResponseMessage.Content.ReadAsStringAsync())!;
                ProjectList.ItemsSource = projects.Select(x => new
                {
                    ProjectName = x.NameProject,
                    RoleInProject = $"{UserAutorizationTrue.userLog.Username}: " + x.TitleRole,
                    Id = x.ProjectId
                }).ToList();
            }
        }
    }

    private async void CreateNewProject(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        CreateProjectWindow createProjectWindow = new CreateProjectWindow();
        await createProjectWindow.ShowDialog(this);
        LoadProjects();
    }

    private void OpenProject(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        ProjectVisible.IsVisible = true;
        UserAutorizationTrue.ProjectId = (Guid)(sender as Border).Tag!;
        LoadTasks();
    }

    private async void LoadTasks()
    {
        using (var Client = new HttpClient())
        {
            HttpResponseMessage httpResponseMessage = await Client.GetAsync($"{BaseAddress.Address}Task/Get_tasks?Email={UserAutorizationTrue.userLog.Email}&Password={UserAutorizationTrue.userLog.Password}&ProjectId={UserAutorizationTrue.ProjectId}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string context = await httpResponseMessage.Content.ReadAsStringAsync();
                List<GetTasksModel> projects = JsonConvert.DeserializeObject<List<GetTasksModel>>(context)!;
                ListTasks.ItemsSource = projects.Select(x => new
                {
                    TaskName = x.TitleTask,
                    x.DescriptionTask,
                    StatusName = x.StatusTitle,
                    Id = x.Id,
                    Color = new SolidColorBrush(
                        Avalonia.Media.Color.FromArgb(
                            System.Drawing.Color.FromName(
                                x.StatusColor!).A,
                            System.Drawing.Color.FromName(
                                x.StatusColor!).R,
                            System.Drawing.Color.FromName(
                                x.StatusColor!).G,
                            System.Drawing.Color.FromName(
                                x.StatusColor!).B)
                        )
                }).ToList();
            }
        }
    }

    private async void EditTaskButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        ProjectVisible.IsVisible = true;
        int idTask = (int)(sender as Button).Tag!;
        CreateNewTaskWindow createNewTaskWindow = new CreateNewTaskWindow(idTask);
        await createNewTaskWindow.ShowDialog(this);
        LoadTasks();
    }

    private async void NewTaskButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        CreateNewTaskWindow createNewTaskWindow = new CreateNewTaskWindow();
        await createNewTaskWindow.ShowDialog(this);
        LoadTasks();
    }
}