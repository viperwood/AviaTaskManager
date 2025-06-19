using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TaskMonagerAviaProject.Models;
using TaskMonagerAviaProject.StaticObjects;

namespace TaskMonagerAviaProject;

public partial class CreateNewTaskWindow : Window
{
    public CreateNewTaskWindow()
    {
        InitializeComponent();
        TextWindow.Text = "Создание новой задачи";
        LoadInfo();
    }

    private int _taskId = -1;

    public CreateNewTaskWindow( int TaskId)
    {
        InitializeComponent();
        TextWindow.Text = "Редактирование задачи";
        _taskId = TaskId;
        LoadTask();
    }

    private HttpResponseMessage httpResponseMessage;

    private async void LoadInfo()
    {
        using (var Client = new HttpClient())
        {
            httpResponseMessage = await Client.GetAsync($"{BaseAddress.Address}Project/Get_users_project?Email={UserAutorizationTrue.userLog.Email}&Password={UserAutorizationTrue.userLog.Password}&ProjectId={UserAutorizationTrue.ProjectId}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string context = await httpResponseMessage.Content.ReadAsStringAsync();
                users = JsonConvert.DeserializeObject<List<GetUsersProject>>(context)!;
                BoxUsersTask.ItemsSource = users.Select(x => new
                {
                    UserName = x.username,
                    Id = x.userId,
                }).ToList();
            }
            httpResponseMessage = await Client.GetAsync($"{BaseAddress.Address}Task/Get_tasks?Email={UserAutorizationTrue.userLog.Email}&Password={UserAutorizationTrue.userLog.Password}&ProjectId={UserAutorizationTrue.ProjectId}&TaskId={_taskId}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string context = await httpResponseMessage.Content.ReadAsStringAsync();
                tasks = JsonConvert.DeserializeObject<List<GetTaskModel>>(context)!;
                BoxTasks.ItemsSource = tasks.ToList();
                /*if (tasks.Count() == 0)
                {
                    TaskList.Clear();
                    loadList();
                }*/
            }
        }
    }

    private List<UserListModel> UserList = new List<UserListModel>();
    private List<GetUsersProject> users = new List<GetUsersProject>();
    private List<TaskModel> TaskList = new List<TaskModel>();
    private List<GetTaskModel> tasks = new List<GetTaskModel>();

    private async void LoadTask()
    {
        using (var Client = new HttpClient())
        {
            httpResponseMessage = await Client.GetAsync($"{BaseAddress.Address}Task/Get_task?Email={UserAutorizationTrue.userLog.Email}&Password={UserAutorizationTrue.userLog.Password}&TaskId={_taskId}&ProjectId={UserAutorizationTrue.ProjectId}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                GetTaskModel projects = JsonConvert.DeserializeObject<List<GetTaskModel>>(await httpResponseMessage.Content.ReadAsStringAsync())![0];
                UserList = projects.Users!;
                TaskList = projects.Tasks!;
                NameTaskText.Text = projects.TitleTask;
                DescriptionTaskText.Text = projects.DescriptionTask;
                loadList();
            }
            LoadInfo();
        }
    }

    private void loadList()
    {
        ListUsersTask.ItemsSource = UserList.Select(x => new
        {
            UsersName = x.Username,
            x.Id
        }).ToList();
        ListTasks.ItemsSource = TaskList.Select(x => new
        {
            x.TitleTask,
            x.Id
        }).ToList();
    }

    private void AddUserButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (BoxUsersTask.SelectedIndex != -1)
        {
            if (UserList.Where(x => x.Id == users[BoxUsersTask.SelectedIndex].userId).Count() == 0)
            {
                UserListModel userListModel = new UserListModel();
                userListModel.Id = users[BoxUsersTask.SelectedIndex].userId;
                userListModel.Username = users[BoxUsersTask.SelectedIndex].username;
                UserList.Add(userListModel);
                loadList();
            }
        }
    }

    private async void SaveTaskButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        EditTaskModel editTaskModel = new EditTaskModel();
        editTaskModel.TitleTask = NameTaskText.Text!;
        editTaskModel.DescriptionTask = DescriptionTaskText.Text!;
        editTaskModel.ProjectId = UserAutorizationTrue.ProjectId;
        editTaskModel.Password = UserAutorizationTrue.userLog.Password;
        editTaskModel.Email = UserAutorizationTrue.userLog.Email;
        if (UserList != null)
        {
            List<int> idUsers = new List<int>();
            foreach (var element in UserList)
            {
                idUsers.Add(element.Id);
            }
            editTaskModel.ExecutorId = idUsers;
        }
        if (TaskList != null)
        {
            List<int> idTasks = new List<int>();
            foreach (var element in TaskList)
            {
                idTasks.Add(element.Id);
            }
            editTaskModel.TasksId = idTasks;
        }
        if (_taskId != -1)
        {
            editTaskModel.TaskId = _taskId;
            using (var Client = new HttpClient())
            {
                httpResponseMessage = await Client.PutAsJsonAsync($"{BaseAddress.Address}Task/Edit_task", editTaskModel);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    ErrorText.Foreground = Brushes.Green;
                }
                else
                {
                    ErrorText.Foreground = Brushes.Red;
                }
                ErrorText.Text = await httpResponseMessage.Content.ReadAsStringAsync();
            }
        }
        else
        {
            using (var Client = new HttpClient())
            {
                httpResponseMessage = await Client.PostAsJsonAsync($"{BaseAddress.Address}Task/Creating_task", editTaskModel);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    ErrorText.Foreground = Brushes.Green;
                }
                else
                {
                    ErrorText.Foreground = Brushes.Red;
                }
                ErrorText.Text = await httpResponseMessage.Content.ReadAsStringAsync();
            }
        }
        LoadInfo();
        await Task.Delay(1500);
        ErrorText.Foreground = Brushes.Red;
        ErrorText.Text = "";
    }

    private void AddTaskButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (BoxTasks.SelectedIndex != -1)
        {
            if (TaskList.Where(x => x.Id == tasks[BoxTasks.SelectedIndex].Id).Count() == 0)
            {
                TaskModel taskModel = new TaskModel();
                taskModel.Id = tasks[BoxTasks.SelectedIndex].Id;
                taskModel.TitleTask = tasks[BoxTasks.SelectedIndex].TitleTask;
                TaskList.Add(taskModel);
                loadList();
            }
        }
    }

    private void DropUsersFromListButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        int id = (int)(sender as Button).Tag!;
        UserList.Remove(UserList.FirstOrDefault(x => x.Id == id)!);
        loadList();
    }

    private void DropTaskFromListButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        int id = (int)(sender as Button).Tag!;
        TaskList.Remove(TaskList.FirstOrDefault(x => x.Id == id)!);
        loadList();
    }
}