using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Threading.Tasks;
using TaskMonagerAviaProject.Models;
using TaskMonagerAviaProject.StaticObjects;

namespace TaskMonagerAviaProject;

public partial class CreateSprintWindow : Window
{
    public CreateSprintWindow()
    {
        InitializeComponent();
        NameWindow.Text = "Создание спринта";
        LoadInfoBox();
    }
    public CreateSprintWindow(int Id)
    {
        InitializeComponent();
        NameWindow.Text = "Редактирование спринта";
        _Id = Id;
        LoadInfo();
    }

    private int _Id = 0;

    private async void LoadInfo()
    {
        using (var Client = new HttpClient())
        {
            HttpResponseMessage httpResponseMessage = await Client.GetAsync($"{BaseAddress.Address}Sprint/Get_sprint_edit?Email={UserAutorizationTrue.userLog.Email}&Password={UserAutorizationTrue.userLog.Password}&ProjectId={UserAutorizationTrue.ProjectId}&SprintId={_Id}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string context = await httpResponseMessage.Content.ReadAsStringAsync();
                GetEditSprintModel getEditSprintModel = JsonConvert.DeserializeObject<GetEditSprintModel>(context)!;
                DateStart.SelectedDate = getEditSprintModel.DateStart;
                DateEnd.SelectedDate = getEditSprintModel.DateEnd;
                tasksList = getEditSprintModel.Tasks!;
                LoadInfoBox();
                loadList();
            }
        }
    }

    private async void CreateSprintButtonAsync(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        
        if (DateStart.SelectedDate != null && DateEnd.SelectedDate != null && tasksList.Count != 0)
        {
            if (_Id == 0)
            {
                using (var Client = new HttpClient())
                {
                    CreateSprintModel createSprintModel = new CreateSprintModel();
                    createSprintModel.DateStart = DateStart.SelectedDate.Value;
                    createSprintModel.DateEnd = DateEnd.SelectedDate.Value;
                    createSprintModel.ProjectId = UserAutorizationTrue.ProjectId;
                    createSprintModel.IdTasks = tasksList.Select(x => x.Id).ToList();
                    createSprintModel.Email = UserAutorizationTrue.userLog.Email;
                    createSprintModel.Password = UserAutorizationTrue.userLog.Password;
                    HttpResponseMessage httpResponseMessage = await Client.PostAsJsonAsync($"{BaseAddress.Address}Sprint/Create_sprint", createSprintModel);
                    if (httpResponseMessage.IsSuccessStatusCode)
                    {
                        ErrorText.Text = await httpResponseMessage.Content.ReadAsStringAsync();
                        ErrorText.Foreground = Avalonia.Media.Brushes.Green;
                        await Task.Delay(1500);
                        Close();
                    }
                    else
                    {
                        ErrorText.Text = await httpResponseMessage.Content.ReadAsStringAsync();
                        ErrorText.Foreground = Avalonia.Media.Brushes.Red;
                        await Task.Delay(1500);
                        ErrorText.Text = "";
                    }
                }

            }
            else
            {
                using (var Client = new HttpClient())
                {
                    CreateSprintModel createSprintModel = new CreateSprintModel();
                    createSprintModel.DateStart = DateStart.SelectedDate.Value;
                    createSprintModel.DateEnd = DateEnd.SelectedDate.Value;
                    createSprintModel.SprintId = _Id;
                    createSprintModel.ProjectId = UserAutorizationTrue.ProjectId;
                    createSprintModel.IdTasks = tasksList.Select(x => x.Id).ToList();
                    createSprintModel.Email = UserAutorizationTrue.userLog.Email;
                    createSprintModel.Password = UserAutorizationTrue.userLog.Password;
                    HttpResponseMessage httpResponseMessage = await Client.PutAsJsonAsync($"{BaseAddress.Address}Sprint/Edit_sprint", createSprintModel);
                    if (httpResponseMessage.IsSuccessStatusCode)
                    {
                        ErrorText.Text = await httpResponseMessage.Content.ReadAsStringAsync();
                        ErrorText.Foreground = Avalonia.Media.Brushes.Green;
                        await Task.Delay(1500);
                        Close();
                    }
                    else
                    {
                        ErrorText.Text = await httpResponseMessage.Content.ReadAsStringAsync();
                        ErrorText.Foreground = Avalonia.Media.Brushes.Red;
                        await Task.Delay(1500);
                        ErrorText.Text = "";
                    }
                }
            }
        }
    }

    private void AddTaskButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (BoxTasks.SelectedIndex != -1)
        {
            if (tasksList.Where(x => x.Id == tasksBox[BoxTasks.SelectedIndex].Id).Count() == 0)
            {
                TaskModel taskModel = new TaskModel();
                taskModel.Id = tasksBox[BoxTasks.SelectedIndex].Id;
                taskModel.TitleTask = tasksBox[BoxTasks.SelectedIndex].TitleTask;
                tasksList.Add(taskModel);
                loadList();
            }
        }
    }

    private void DropTaskFromListButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        int id = (int)(sender as Button).Tag!;
        tasksList.Remove(tasksList.FirstOrDefault(x => x.Id == id)!);
        loadList();
    }

    private List<GetTaskModel> tasksBox = new List<GetTaskModel>();
    private List<TaskModel> tasksList = new List<TaskModel>();

    private async void LoadInfoBox()
    {
        using (var Client = new HttpClient())
        {
            HttpResponseMessage httpResponseMessage = await Client.GetAsync($"{BaseAddress.Address}Task/Get_tasks_box?Email={UserAutorizationTrue.userLog.Email}&Password={UserAutorizationTrue.userLog.Password}&ProjectId={UserAutorizationTrue.ProjectId}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string context = await httpResponseMessage.Content.ReadAsStringAsync();
                tasksBox = JsonConvert.DeserializeObject<List<GetTaskModel>>(context)!;
                BoxTasks.ItemsSource = tasksBox.ToList();
                if (tasksBox.Count() == 0)
                {
                    tasksList.Clear();
                    loadList();
                }
            }
        }
    }
    private void loadList()
    {
        ListTasks.ItemsSource = tasksList.Select(x => new
        {
            x.TitleTask,
            x.Id
        }).ToList();
    }

}