using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using Avalonia.Media;
using System.IO;
using System.Linq;
using System.Net.Http;
using TaskMonagerAviaProject.Models;
using TaskMonagerAviaProject.StaticObjects;
using System.Threading.Tasks;
using System.Net.Http.Json;
using Metsys.Bson;
using static System.Net.Mime.MediaTypeNames;
using SkiaSharp;
using Avalonia.Input;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TaskMonagerAviaProject;

public partial class WindowProjects : Window
{
    public WindowProjects()
    {
        InitializeComponent();
        _content = File.ReadAllText(path);
        ImageTest.Source = new Avalonia.Media.Imaging.Bitmap(AppDomain.CurrentDomain.BaseDirectory + @"\Images\1667650479147654979.jpg");/*
        loadTestInfo();*/
        AddHandler(DragDrop.DropEvent, Drop);
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
        LoadProjects();/*
        loadTestInfo();*/
        AddHandler(DragDrop.DropEvent, Drop);
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
        if (_buttonRightPanel == 1)
        {
            _buttonRightPanel = 0;
        }
        else
        {
            _buttonRightPanel = 1;
            LoadGetProposalsJoinProjects();
        }
        RightPanel();
    }


    private async void LoadGetProposalsJoinProjects()
    {
        UserName.Text = UserAutorizationTrue.userLog.Username + "#" + UserAutorizationTrue.userLog.Id;
        if (UserAutorizationTrue.userLog.UserImage != null)
        {
            using (MemoryStream memoryStream = new MemoryStream(UserAutorizationTrue.userLog.UserImage))
            {
                ImageTest.Source = new Avalonia.Media.Imaging.Bitmap(memoryStream);
            }
        }
        using (var Client = new HttpClient())
        {
            HttpResponseMessage httpResponseMessage = await Client.GetAsync($"{BaseAddress.Address}Project/Get_proposals_join_projects?Email={UserAutorizationTrue.userLog.Email}&Password={UserAutorizationTrue.userLog.Password}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                List<ProjectsModel> ProposalsJoinProjects = JsonConvert.DeserializeObject<List<ProjectsModel>>(await httpResponseMessage.Content.ReadAsStringAsync())!;
                ListAcceptanceCancelProjects.ItemsSource = ProposalsJoinProjects.ToList();
            }
        }
    }


    private byte _buttonRightPanel = 0;
    private byte _buttonRightProjectPanel = 0;

    private void RightPanel()
    {
        switch (_buttonRightPanel)
        {
            case 0:
                Panel.IsPaneOpen = false;
                UserInfoButton.Background = Avalonia.Media.Brushes.DimGray;
                MailBoxButton.Background = Avalonia.Media.Brushes.DimGray;
                UserFrendsButtonP.Background = Avalonia.Media.Brushes.DimGray;
                UserInfoPanel.IsVisible = false;
                MailBoxPanel.IsVisible = false;
                FrendsPanel.IsVisible = false;
                break;
            case 1:
                Panel.IsPaneOpen = true;
                Panel.OpenPaneLength = 600;
                UserInfoButton.Background = Avalonia.Media.Brushes.LightGray;
                MailBoxButton.Background = Avalonia.Media.Brushes.DimGray;
                UserFrendsButtonP.Background = Avalonia.Media.Brushes.DimGray;
                UserInfoPanel.IsVisible = true;
                MailBoxPanel.IsVisible = false;
                FrendsPanel.IsVisible = false;
                break;
            case 2:
                Panel.IsPaneOpen = true;
                Panel.OpenPaneLength = 800;
                UserInfoButton.Background = Avalonia.Media.Brushes.DimGray;
                MailBoxButton.Background = Avalonia.Media.Brushes.LightGray;
                UserFrendsButtonP.Background = Avalonia.Media.Brushes.DimGray;
                UserInfoPanel.IsVisible = false;
                MailBoxPanel.IsVisible = true;
                FrendsPanel.IsVisible = false;
                break;
            case 3:
                Panel.IsPaneOpen = true;
                Panel.OpenPaneLength = 600;
                UserInfoButton.Background = Avalonia.Media.Brushes.DimGray;
                MailBoxButton.Background = Avalonia.Media.Brushes.DimGray;
                UserFrendsButtonP.Background = Avalonia.Media.Brushes.LightGray;
                UserInfoPanel.IsVisible = false;
                MailBoxPanel.IsVisible = false;
                FrendsPanel.IsVisible = true;
                break;
        }
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

    private List<ProjectsModel> projectsList = new List<ProjectsModel>();

    private async void LoadProjects()
    {
        using (var Client = new HttpClient())
        {
            HttpResponseMessage httpResponseMessage = await Client.GetAsync($"{BaseAddress.Address}Project/Get_projects?Email={UserAutorizationTrue.userLog.Email}&Password={UserAutorizationTrue.userLog.Password}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                projectsList = JsonConvert.DeserializeObject<List<ProjectsModel>>(await httpResponseMessage.Content.ReadAsStringAsync())!;
                ProjectList.ItemsSource = projectsList.Select(x => new
                {
                    ProjectName = x.NameProject,
                    RoleInProject = $"{UserAutorizationTrue.userLog.Username}: " + x.TitleRole,
                    Id = x.ProjectId
                }).ToList();
                if (_buttonRightPanel == 1)
                {
                    LoadGetProposalsJoinProjects();
                }
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
        UserAutorizationTrue.ProjectId = (Guid)(sender as Border).Tag!;
        LoadTasks();
        ProjectVisible.IsVisible = true;
        MenuButtonsProject.IsVisible = true;
        TasksSprintsMenu();
    }

    private async void LoadTasks()
    {
        using (var Client = new HttpClient())
        {
            HttpResponseMessage httpResponseMessage = await Client.GetAsync($"{BaseAddress.Address}Task/Get_tasks_tree?Email={UserAutorizationTrue.userLog.Email}&Password={UserAutorizationTrue.userLog.Password}&ProjectId={UserAutorizationTrue.ProjectId}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string context = await httpResponseMessage.Content.ReadAsStringAsync();
                List<GetTasksModel> tasksModel = JsonConvert.DeserializeObject<List<TasksProject>>(context)!.Select(x => new GetTasksModel(x)).ToList();
                ListTasks.ItemsSource = tasksModel.ToList();
                ProjectVisible.IsVisible = true;
                RightProjectPanel();
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


    private void ButtonMailBoxPanel(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (_buttonRightPanel == 2)
        {
            _buttonRightPanel = 0;
        }
        else
        {
            _buttonRightPanel = 2;
            LoadFrends();
        }
        RightPanel();
    }

    private List<GetFrendsList> projects = new List<GetFrendsList>();
    private List<GetMalsUsersModel> malsUsers = new List<GetMalsUsersModel>();
    private List<GetComentProjectModel> malsUsersProject = new List<GetComentProjectModel>();

    private async void LoadFrends()
    {
        using (var Client = new HttpClient())
        {
            HttpResponseMessage httpResponseMessage = await Client.GetAsync($"{BaseAddress.Address}Frend/List_frend?Email={UserAutorizationTrue.userLog.Email}&Password={UserAutorizationTrue.userLog.Password}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string context = await httpResponseMessage.Content.ReadAsStringAsync();
                projects = JsonConvert.DeserializeObject<List<GetFrendsList>>(context)!.ToList();
                if (_buttonRightPanel == 2)
                {
                    ListFrends.ItemsSource = projects.ToList();
                }
                else if (_buttonRightPanel == 3)
                {
                    ListUserFrends.ItemsSource = projects.ToList();
                    LoadAddFrendsList();
                }
            }
        }
    }

    private int IdFrend;

    private async void SelectFrend(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        IdFrend = (int)(sender as Border).Tag!;
        FrendName.Text = projects.FirstOrDefault(x => x.FrendId == IdFrend)!.FrendName;
        LoadMailBox();
    }

    private async void LoadMailBox()
    {
        using (var Client = new HttpClient())
        {
            HttpResponseMessage httpResponseMessage = await Client.GetAsync($"{BaseAddress.Address}MailBox/Conclusion_messages?Email={UserAutorizationTrue.userLog.Email}&Password={UserAutorizationTrue.userLog.Password}&DestinationId={IdFrend}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string context = await httpResponseMessage.Content.ReadAsStringAsync();
                malsUsers = JsonConvert.DeserializeObject<List<GetMalsUsersModel>>(context)!.ToList();
                malsUsers = malsUsers.OrderBy(x => x.DateAndTimeMail).ToList();
                ListTextMails.ItemsSource = malsUsers.Select(x => new
                {
                    x.Id,
                    x.AnswerMailId,
                    AnswerMailText = "Ответ на: «" + x.AnswerMailText + "»",
                    x.TextMail,
                    x.TitleStatus,
                    x.DateAndTimeMail,
                    VisibleAnsver = x.AnswerMailText != "" ? true : false,
                    HorizontalAligmentText = x.AddresseeId == IdFrend ? "Left" : "Right",
                    ColorUser = x.AddresseeId == IdFrend ? Avalonia.Media.Brushes.Gray : Avalonia.Media.Brushes.DimGray
                }).ToList();
                PushTextPanel.IsVisible = true;
            }
        }
    }

    private int idEditMailText = 0;
    private int idAnswerMailText = 0;
    private bool OpenPanelEditOrAnswer = false;

    private async void PushTextMailButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(SecondPasswordTextReg.Text))
        {
            using (var Client = new HttpClient())
            {
                if (idEditMailText == 0)
                {
                    SendingMessageModel sendingMessageModel = new SendingMessageModel();
                    sendingMessageModel.Email = UserAutorizationTrue.userLog.Email;
                    sendingMessageModel.Password = UserAutorizationTrue.userLog.Password;
                    sendingMessageModel.TextMail = SecondPasswordTextReg.Text!;
                    sendingMessageModel.DestinationId = IdFrend!;
                    sendingMessageModel.AnswerId = idAnswerMailText!;
                    HttpResponseMessage httpResponseMessage = await Client.PostAsJsonAsync($"{BaseAddress.Address}MailBox/Sending_message", sendingMessageModel);
                    if (httpResponseMessage.IsSuccessStatusCode)
                    {
                        LoadMailBox();
                        SecondPasswordTextReg.Text = "";
                        idAnswerMailText = 0;
                        PanelAnswer.IsVisible = false;
                        OpenPanelEditOrAnswer = false;
                    }
                }
                else
                {
                    EditingMessageModel editingMessageModel = new EditingMessageModel();
                    editingMessageModel.MessageId = idEditMailText;
                    editingMessageModel.TextMail = SecondPasswordTextReg.Text!;
                    editingMessageModel.Email = UserAutorizationTrue.userLog.Email;
                    editingMessageModel.Password = UserAutorizationTrue.userLog.Password;
                    HttpResponseMessage httpResponseMessage = await Client.PutAsJsonAsync($"{BaseAddress.Address}MailBox/Editing_message", editingMessageModel);
                    if (httpResponseMessage.IsSuccessStatusCode)
                    {
                        LoadMailBox();
                        SecondPasswordTextReg.Text = "";
                        idEditMailText = 0;
                        PanelEdit.IsVisible = false;
                        OpenPanelEditOrAnswer = false;
                    }
                }
                
            }
        }
    }

    private void EditMailTextButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (!OpenPanelEditOrAnswer)
        {
            idEditMailText = (int)(sender as MenuItem).Tag!;
            if (malsUsers.Where(x => x.Id == idEditMailText && x.AddresseeId == IdFrend).Count() == 0)
            {
                PanelEdit.IsVisible = true;
                OpenPanelEditOrAnswer = true;
                SecondPasswordTextReg.Text = malsUsers.FirstOrDefault(x => x.Id == idEditMailText)!.TextMail;
            }
        }
    }

    private async void DelitMailTextButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        int id = (int)(sender as MenuItem).Tag!;
        using (var Client = new HttpClient())
        {
            HttpResponseMessage httpResponseMessage = await Client.DeleteAsync($"{BaseAddress.Address}MailBox/Delete_message?Email={UserAutorizationTrue.userLog.Email}&Password={UserAutorizationTrue.userLog.Password}&MessageId={id}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                LoadMailBox();
            }
        }
    }

    private void AnswerMailTextButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (!OpenPanelEditOrAnswer)
        {
            idAnswerMailText = (int)(sender as MenuItem).Tag!;
            PanelAnswer.IsVisible = true;
            OpenPanelEditOrAnswer = true;
            GetMalsUsersModel getMalsUsersModel = malsUsers.FirstOrDefault(x => x.Id == idAnswerMailText)!;
            AnswerTextMail.Text = getMalsUsersModel.TextMail;
            AnswerTitleStatus.Text = getMalsUsersModel.TitleStatus;
            AnswerDateAndTimeMail.Text = getMalsUsersModel.DateAndTimeMail.ToString();
        }
    }

    private void CloseAnswer(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        PanelAnswer.IsVisible = false;
        OpenPanelEditOrAnswer = false;
        idAnswerMailText = 0;
    }

    private void CloseEdit(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        PanelEdit.IsVisible = false;
        OpenPanelEditOrAnswer = false;
        idEditMailText = 0;
    }

    private async void EditProjectButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        CreateProjectWindow createProjectWindow = new CreateProjectWindow(UserAutorizationTrue.ProjectId);
        await createProjectWindow.ShowDialog(this);
        if(UserAutorizationTrue.ProjectId == Guid.Empty)
        {
            ProjectVisible.IsVisible = false;
        }
        LoadProjects();
    }

    private void AddFrendButtonPanel(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (_buttonRightPanel == 3)
        {
            _buttonRightPanel = 0;
        }
        else
        {
            _buttonRightPanel = 3;
        }
        RightPanel();
        LoadFrends();
    }

    private async void LoadAddFrendsList()
    {
        using (var Client = new HttpClient())
        {
            HttpResponseMessage httpResponseMessage = await Client.GetAsync($"{BaseAddress.Address}Frend/List_proposal?Email={UserAutorizationTrue.userLog.Email}&Password={UserAutorizationTrue.userLog.Password}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string context = await httpResponseMessage.Content.ReadAsStringAsync();
                List<GetAddFrendsList> addFrendsLists = JsonConvert.DeserializeObject<List<GetAddFrendsList>>(context)!.ToList();
                ListAddUserFrends.ItemsSource = addFrendsLists.ToList();
            }
        }
    }

    private void CancelFrendButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        int Id = (int)(sender as Button).Tag!;
        CancelFrend(Id);
    }

    private async void CancelFrend(int Id)
    {
        using (var Client = new HttpClient())
        {
            AcceptanceCancelModel acceptanceCancelModel = new AcceptanceCancelModel();
            acceptanceCancelModel.Id = Id;
            HttpResponseMessage httpResponseMessage = await Client.DeleteAsync($"{BaseAddress.Address}Frend/Cancel_frend?Email={UserAutorizationTrue.userLog.Email}&Password={UserAutorizationTrue.userLog.Password}&Id={acceptanceCancelModel.Id}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                LoadFrends();
            }
        }
    }

    private async void AcceptanceFrend(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        int Id = (int)(sender as Button).Tag!;
        using (var Client = new HttpClient())
        {
            AcceptanceCancelModel acceptanceCancelModel = new AcceptanceCancelModel();
            acceptanceCancelModel.Id = Id;
            acceptanceCancelModel.Email = UserAutorizationTrue.userLog.Email;
            acceptanceCancelModel.Password = UserAutorizationTrue.userLog.Password;
            HttpResponseMessage httpResponseMessage = await Client.PutAsJsonAsync($"{BaseAddress.Address}Frend/Acceptance_frend", acceptanceCancelModel);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                LoadFrends();
            }
        }
    }

    private async void AddNewFrend(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(FrendNameText.Text))
        {
            using (var Client = new HttpClient())
            {
                ProposalFrendModel proposalFrendModel = new ProposalFrendModel();
                proposalFrendModel.Frend = FrendNameText.Text;
                proposalFrendModel.Email = UserAutorizationTrue.userLog.Email;
                proposalFrendModel.Password = UserAutorizationTrue.userLog.Password;
                HttpResponseMessage httpResponseMessage = await Client.PostAsJsonAsync($"{BaseAddress.Address}Frend/Proposal_frend", proposalFrendModel);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    ErrorAddFrend.Foreground = Avalonia.Media.Brushes.Green;
                }
                else
                {
                    ErrorAddFrend.Foreground = Avalonia.Media.Brushes.Red;
                }
                ErrorAddFrend.Text = await httpResponseMessage.Content.ReadAsStringAsync();
                await Task.Delay(3000);
                ErrorAddFrend.Text = "";
            }
        }
    }

    private void DeleteFrendButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        int Id = (int)(sender as MenuItem).Tag!;
        CancelFrend(Id);
    }

    private void RightProjectPanel()
    {
        switch (_buttonRightProjectPanel)
        {
            case 0:
                PanelProject.IsPaneOpen = false;
                ButtonProjectRightPanelInfo.Background = Avalonia.Media.Brushes.DimGray;
                ButtonProjectRightPanelMail.Background = Avalonia.Media.Brushes.DimGray;
                PanelProjectInfo.IsVisible = false;
                PanelProjectMails.IsVisible = false;
                break;
            case 1:
                PanelProject.IsPaneOpen = true;
                PanelProject.OpenPaneLength = 600;
                ButtonProjectRightPanelInfo.Background = Avalonia.Media.Brushes.LightGray;
                ButtonProjectRightPanelMail.Background = Avalonia.Media.Brushes.DimGray;
                PanelProjectInfo.IsVisible = true;
                PanelProjectMails.IsVisible = false;
                LoadInfoProject();
                break;
            case 2:
                PanelProject.IsPaneOpen = true;
                PanelProject.OpenPaneLength = 800;
                ButtonProjectRightPanelInfo.Background = Avalonia.Media.Brushes.DimGray;
                ButtonProjectRightPanelMail.Background = Avalonia.Media.Brushes.LightGray;
                PanelProjectInfo.IsVisible = false;
                PanelProjectMails.IsVisible = true;
                LoadMailProject();
                break;
        }
    }

    private void ProjectRightPanelInfo(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (_buttonRightProjectPanel == 1)
        {
            _buttonRightProjectPanel = 0;
        }
        else
        {
            _buttonRightProjectPanel = 1;
        }
        RightProjectPanel();
    }

    private void ProjectRightPanelMails(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (_buttonRightProjectPanel == 2)
        {
            _buttonRightProjectPanel = 0;
        }
        else
        {
            _buttonRightProjectPanel = 2;
        }
        RightProjectPanel();
    }


    private async void LoadMailProject()
    {
        using (var Client = new HttpClient())
        {
            AcceptanceCancelModel acceptanceCancelModel = new AcceptanceCancelModel();

            HttpResponseMessage httpResponseMessage = await Client.GetAsync($"{BaseAddress.Address}ComentsProjects/Conclusion_coment_project?Email={UserAutorizationTrue.userLog.Email}&Password={UserAutorizationTrue.userLog.Password}&ProjectId={UserAutorizationTrue.ProjectId}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                NameProjectMail.Text = projectsList.FirstOrDefault(x => x.ProjectId == UserAutorizationTrue.ProjectId)!.NameProject;
                string context = await httpResponseMessage.Content.ReadAsStringAsync();
                malsUsersProject = JsonConvert.DeserializeObject<List<GetComentProjectModel>>(context)!.ToList();
                malsUsersProject = malsUsersProject.OrderBy(x => x.DateAndTimeComent).ToList();
                ListTextMailsProject.ItemsSource = malsUsersProject.Select(x => new
                {
                    x.Id,
                    x.AnswerMailId,
                    VisibleAnsver = x.AnswerMailText != "" ? true : false,
                    AnswerMailText = $"{x.AnswerMailUser}: «" + x.AnswerMailText + "»",
                    x.TextMail,
                    x.Username,
                    x.AnswerMailUser,
                    TitleStatusDateAndTimeComent = x.TitleStatus + " " + x.DateAndTimeComent,
                    HorizontalAligmentText = x.UserId == UserAutorizationTrue.userLog.Id ? "Right" : "Left",
                    ColorUser = x.UserId == UserAutorizationTrue.userLog.Id ? Avalonia.Media.Brushes.DimGray : Avalonia.Media.Brushes.Gray
                }).ToList();

                httpResponseMessage = await Client.GetAsync($"{BaseAddress.Address}Project/Get_users_project?Email={UserAutorizationTrue.userLog.Email}&Password={UserAutorizationTrue.userLog.Password}&ProjectId={UserAutorizationTrue.ProjectId}");
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    context = await httpResponseMessage.Content.ReadAsStringAsync();
                    List<GetUsersProjectModel> getUsersProjectModel = JsonConvert.DeserializeObject<List<GetUsersProjectModel>>(context)!.ToList();
                    ListUsersProjects.ItemsSource = getUsersProjectModel.ToList();
                }
            }
        }
    }

    private void AnswerMailTextProjectButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (!OpenPanelEditOrAnswerProject)
        {
            idAnswerMailProjectText = (int)(sender as MenuItem).Tag!;
            PanelAnswerProject.IsVisible = true;
            OpenPanelEditOrAnswerProject = true;
            GetComentProjectModel getMalsUsersModel = malsUsersProject.FirstOrDefault(x => x.Id == idAnswerMailProjectText)!;
            AnswerTextMailProject.Text = getMalsUsersModel.TextMail;
            AnswerTitleStatusProject.Text = getMalsUsersModel.TitleStatus;
            UsernameTextMailProject.Text = getMalsUsersModel.Username;
            AnswerDateAndTimeMailProject.Text = getMalsUsersModel.DateAndTimeComent.ToString();
        }
    }

    private void ProjectCloseAnswer(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        PanelAnswerProject.IsVisible = false;
        OpenPanelEditOrAnswerProject = false;
        idAnswerMailProjectText = 0;
    }

    private void ProjectCloseEdit(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        PanelEditTextProject.IsVisible = false;
        OpenPanelEditOrAnswerProject = false;
        idEditMailProjectText = 0;
    }

    private int idEditMailProjectText = 0;
    private int idAnswerMailProjectText = 0;
    private bool OpenPanelEditOrAnswerProject = false;

    private async void PushTextMailProjectButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(PushTextMail.Text))
        {
            using (var Client = new HttpClient())
            {
                if (idEditMailProjectText == 0)
                {
                    CreateComentProjectModel createComentProjectModel = new CreateComentProjectModel();
                    createComentProjectModel.Email = UserAutorizationTrue.userLog.Email;
                    createComentProjectModel.Password = UserAutorizationTrue.userLog.Password;
                    createComentProjectModel.TextMail = PushTextMail.Text!;
                    createComentProjectModel.ProjectId = UserAutorizationTrue.ProjectId;
                    createComentProjectModel.AnswerId = idAnswerMailProjectText!;
                    HttpResponseMessage httpResponseMessage = await Client.PostAsJsonAsync($"{BaseAddress.Address}ComentsProjects/Create_coment_project", createComentProjectModel);
                    if (httpResponseMessage.IsSuccessStatusCode)
                    {
                        LoadMailProject();
                        PushTextMail.Text = "";
                        idAnswerMailProjectText = 0;
                        PanelAnswerProject.IsVisible = false;
                        OpenPanelEditOrAnswerProject = false;
                    }
                }
                else
                {
                    EditingComentProjectModel editingComentProjectModel = new EditingComentProjectModel();
                    editingComentProjectModel.MessageId = idEditMailProjectText;
                    editingComentProjectModel.TextMail = PushTextMail.Text!;
                    editingComentProjectModel.ProjectId = UserAutorizationTrue.ProjectId;
                    editingComentProjectModel.Email = UserAutorizationTrue.userLog.Email;
                    editingComentProjectModel.Password = UserAutorizationTrue.userLog.Password;
                    HttpResponseMessage httpResponseMessage = await Client.PutAsJsonAsync($"{BaseAddress.Address}ComentsProjects/Editing_coment_project", editingComentProjectModel);
                    if (httpResponseMessage.IsSuccessStatusCode)
                    {
                        LoadMailProject();
                        PushTextMail.Text = "";
                        idEditMailProjectText = 0;
                        PanelEditTextProject.IsVisible = false;
                        OpenPanelEditOrAnswerProject = false;
                    }
                }

            }
        }
    }

    private void EditMailTextProjectButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (!OpenPanelEditOrAnswerProject)
        {
            idEditMailProjectText = (int)(sender as MenuItem).Tag!;
            if (malsUsersProject.Where(x => x.Id == idEditMailProjectText && x.UserId != UserAutorizationTrue.userLog.Id).Count() == 0)
            {
                PanelEditTextProject.IsVisible = true;
                OpenPanelEditOrAnswerProject = true;
                PushTextMail.Text = malsUsersProject.FirstOrDefault(x => x.Id == idEditMailProjectText)!.TextMail;
            }
        }
    }

    private async void DelitMailTextProjectButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        int id = (int)(sender as MenuItem).Tag!;
        using (var Client = new HttpClient())
        {
            HttpResponseMessage httpResponseMessage = await Client.DeleteAsync($"{BaseAddress.Address}ComentsProjects/Delete_coment_project?Email={UserAutorizationTrue.userLog.Email}&Password={UserAutorizationTrue.userLog.Password}&MessageId={id}&ProjectId={UserAutorizationTrue.ProjectId}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                LoadMailProject();
            }
        }
    }

    private async void Loadsprints()
    {
        using (var Client = new HttpClient())
        {
            HttpResponseMessage httpResponseMessage = await Client.GetAsync($"{BaseAddress.Address}Sprint/Get_sprints?Email={UserAutorizationTrue.userLog.Email}&Password={UserAutorizationTrue.userLog.Password}&ProjectId={UserAutorizationTrue.ProjectId}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string context = await httpResponseMessage.Content.ReadAsStringAsync();
                List<SprintsProjectModel> sprintsProjectModels = JsonConvert.DeserializeObject<List<SprintsProjectModel>>(context)!.ToList();
                ListSprintsproject.ItemsSource = sprintsProjectModels.Select(x => new
                {
                    x.Id,
                    Date = $"С {x.DateStart} по {x.DateEnd}",
                    Status = x.TitleStatus
                }).ToList();
            }
        }
    }
    private async void Loadsprint(int Id)
    {
        using (var Client = new HttpClient())
        {
            HttpResponseMessage httpResponseMessage = await Client.GetAsync($"{BaseAddress.Address}Sprint/Get_sprint?Email={UserAutorizationTrue.userLog.Email}&Password={UserAutorizationTrue.userLog.Password}&ProjectId={UserAutorizationTrue.ProjectId}&SprintId={Id}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string context = await httpResponseMessage.Content.ReadAsStringAsync();
                List<GetSprintModel> getSprintModels = JsonConvert.DeserializeObject<List<GetSprintModel>>(context)!.ToList();
                ListStatusesSprint.ItemsSource = getSprintModels.Select(x => new
                {
                    x.Id,
                    x.TitleStatus,
                    Color = new SolidColorBrush(Avalonia.Media.Color.FromArgb(System.Drawing.ColorTranslator.FromHtml(x.Color).A, System.Drawing.ColorTranslator.FromHtml(x.Color).R, System.Drawing.ColorTranslator.FromHtml(x.Color).G, System.Drawing.ColorTranslator.FromHtml(x.Color).B)),
                    ListTasks = x.Tasks!.Select(y => new
                    {
                        IdTask = y.Id,
                        y.TitleTask
                    })
                }).ToList();
            }
        }
    }














    //Действие тащить
    async void DoDrag(object? sender, PointerPressedEventArgs e)
    {
        //Результат действия тащить
        var result = await DragDrop.DoDragDrop(e, new DataObject(), DragDropEffects.Move);
        switch (result)
        {
            case DragDropEffects.Move:
                int Id = (int)(sender as Control).Tag!;
                break;
            case DragDropEffects.None:
                break;
        }
    }
    //Действие бросить
    void Drop(object? sender, DragEventArgs e)
    {
        var selectElement = e.Source as Control;
        do
        {
            if (selectElement!.GetType().Name == "Grid")
            {
                if (selectElement.Tag != null)
                {
                    break;
                }
            }
            selectElement = selectElement.Parent as Control;
        }
        while (true);
        int Id = (int)selectElement.Tag;
        e.DragEffects = DragDropEffects.Move;
    }
















    private async void LoadInfoProject()
    {
        if (UserAutorizationTrue.ProjectId == Guid.Empty)
        {
            ProjectVisible.IsVisible = false;
        }
        else
        {
            NameProjectText.Text = projectsList.FirstOrDefault(x => x.ProjectId == UserAutorizationTrue.ProjectId)!.NameProject;
            using (var Client = new HttpClient())
            {
                HttpResponseMessage httpResponseMessage = await Client.GetAsync($"{BaseAddress.Address}Project/Get_users_project?Email={UserAutorizationTrue.userLog.Email}&Password={UserAutorizationTrue.userLog.Password}&ProjectId={UserAutorizationTrue.ProjectId}");
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    string context = await httpResponseMessage.Content.ReadAsStringAsync();
                    List<GetUsersProjectModel> getUsersProjectModel = JsonConvert.DeserializeObject<List<GetUsersProjectModel>>(context)!.ToList();
                    UsersProjectList.ItemsSource = getUsersProjectModel.ToList();

                    httpResponseMessage = await Client.GetAsync($"{BaseAddress.Address}Role/Get_role?Email={UserAutorizationTrue.userLog.Email}&Password={UserAutorizationTrue.userLog.Password}&ProjectId={UserAutorizationTrue.ProjectId}");
                    if (httpResponseMessage.IsSuccessStatusCode)
                    {
                        context = await httpResponseMessage.Content.ReadAsStringAsync();
                        List<GetRoleModel> getRoleModel = JsonConvert.DeserializeObject<List<GetRoleModel>>(context)!.ToList();
                        RolesProjectList.ItemsSource = getRoleModel.ToList();
                    }
                }
            }
        }
    }

    private async void DeleteRoleProjectButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        int id = (int)(sender as MenuItem).Tag!;
        using (var Client = new HttpClient())
        {
            HttpResponseMessage httpResponseMessage = await Client.DeleteAsync($"{BaseAddress.Address}Role/Delete_role?Email={UserAutorizationTrue.userLog.Email}&Password={UserAutorizationTrue.userLog.Password}&ProjectId={UserAutorizationTrue.ProjectId}&IdRole={id}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                LoadInfoProject();
            }
        }
    }

    private async void CreateNewRoleProject(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        CreateRoleProjectWindow createRoleProjectWindow = new CreateRoleProjectWindow();
        await createRoleProjectWindow.ShowDialog(this);
        LoadInfoProject();
    }

    private async void AddUserProject(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        AddUserProjectWindow addUserProjectWindow = new AddUserProjectWindow();
        await addUserProjectWindow.ShowDialog(this);
        LoadInfoProject();
    }

    private async void DeleteUserProjectButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        int id = (int)(sender as MenuItem).Tag!;
        using (var Client = new HttpClient())
        {
            HttpResponseMessage httpResponseMessage = await Client.DeleteAsync($"{BaseAddress.Address}Project/Delete_object?Email={UserAutorizationTrue.userLog.Email}&Password={UserAutorizationTrue.userLog.Password}&ProjectId={UserAutorizationTrue.ProjectId}&UserId={id}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                LoadInfoProject();
            }
        }
    }

    private async void AcceptanceProject(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Guid id = (Guid)(sender as Button).Tag!;
        using (var Client = new HttpClient())
        {
            TakingPartProjectModel takingPartProjectModel = new TakingPartProjectModel();
            takingPartProjectModel.ProjectId = id;
            takingPartProjectModel.Email = UserAutorizationTrue.userLog.Email;
            takingPartProjectModel.Password = UserAutorizationTrue.userLog.Password;
            HttpResponseMessage httpResponseMessage = await Client.PutAsJsonAsync($"{BaseAddress.Address}Project/Taking_part_project", takingPartProjectModel);
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                
                LoadProjects();
            }
        }
    }

    private async void CancelProjectButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Guid id = (Guid)(sender as Button).Tag!;
        using (var Client = new HttpClient())
        {
            HttpResponseMessage httpResponseMessage = await Client.DeleteAsync($"{BaseAddress.Address}Project/Rejection_participation_project?Email={UserAutorizationTrue.userLog.Email}&Password={UserAutorizationTrue.userLog.Password}&ProjectId={id}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                LoadGetProposalsJoinProjects();
            }
        }
    }

    private async void NewStatusTaskButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        CreateNewStatusTaskWindow createNewStatusTaskWindow = new CreateNewStatusTaskWindow();
        await createNewStatusTaskWindow.ShowDialog(this);
    }

    private async void EditUseutton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        EditUserLoginWindow editUserLoginWindow = new EditUserLoginWindow();
        await editUserLoginWindow.ShowDialog(this);
        UserName.Text = UserAutorizationTrue.userLog.Username + "#" + UserAutorizationTrue.userLog.Id;
        if (UserAutorizationTrue.userLog.UserImage != null)
        {
            using (MemoryStream memoryStream = new MemoryStream(UserAutorizationTrue.userLog.UserImage))
            {
                ImageTest.Source = new Avalonia.Media.Imaging.Bitmap(memoryStream);
            }
        }
    }

    private void TasksSprintsMenu()
    {
        switch (ProjectMenuButtons)
        {
            case 0:

                break;
            case 1:
                TasksButtonMenu.Background = Avalonia.Media.Brushes.LightGray;
                SprintsButtonMenu.Background = Avalonia.Media.Brushes.DimGray;
                TaskMenu.IsVisible = true;
                SprintMenu.IsVisible = false;
                ListTasks.IsVisible = true;
                SprintsList.IsVisible = false;
                break;
            case 2:
                SprintsButtonMenu.Background = Avalonia.Media.Brushes.LightGray;
                TasksButtonMenu.Background = Avalonia.Media.Brushes.DimGray;
                TaskMenu.IsVisible = false;
                SprintMenu.IsVisible = true;
                ListTasks.IsVisible = false;
                SprintsList.IsVisible = true;
                Loadsprints();
                break;
        }
    }

    private byte ProjectMenuButtons = 1;

    private void SprintsButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        ProjectMenuButtons = 2;
        TasksSprintsMenu();
    }

    private void TasksButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        ProjectMenuButtons = 1;
        TasksSprintsMenu();
    }

    private void SprintLoadButton(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        int id = (int)(sender as Border).Tag!;
        Loadsprint(id);
    }
}