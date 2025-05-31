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
using System.Timers;
using Avalonia.Threading;
using Avalonia.Styling;
using Avalonia;
using Avalonia.Markup.Xaml.Styling;

namespace TaskMonagerAviaProject;

public partial class WindowProjects : Window
{
    public WindowProjects()
    {
        InitializeComponent();
        _content = File.ReadAllText(path);
        ImageTest.Source = new Avalonia.Media.Imaging.Bitmap(AppDomain.CurrentDomain.BaseDirectory + @"\Images\1667650479147654979.jpg");
        LoadStile();
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
        LoadStile();
        LoadProjects();
        AddHandler(DragDrop.DropEvent, Drop);
    }

    private void LoadStile()
    {
        if (UserAutorizationTrue.userLog.ImageBackground != null)
        {
            using (MemoryStream memoryStream = new MemoryStream(UserAutorizationTrue.userLog.ImageBackground!))
            {
                ImageBackground.Source = new Avalonia.Media.Imaging.Bitmap(memoryStream);
            }
        }
        else
        {
            ImageBackground.Source = new Avalonia.Media.Imaging.Bitmap(AppDomain.CurrentDomain.BaseDirectory + @"\Images\testImmage.jpg");
        }
        if (UserAutorizationTrue.userLog.DarckLightColor != null)
        {
            DarckLight = !Convert.ToBoolean(UserAutorizationTrue.userLog.DarckLightColor);
            DayNightStile();
        }
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
        TopButtonVisible.IsVisible = true;
        MenuButtonsProject.IsVisible = true;
        ListTasksUserWindow.IsVisible = false;
        ListStatusesSprint.IsVisible = false;
        ListSprintsproject.IsVisible = false;
        if (ProjectMenuButtons == 2)
        {
            Loadsprints();
        }
        PanelProject.DisplayMode = SplitViewDisplayMode.CompactInline;
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
                    VisibleAnsver = x.AnswerMailText != null ? true : false,
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
            MenuButtonsProject.IsVisible = false;
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
                    VisibleAnsver = x.AnswerMailText != null ? true : false,
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
                sprintsProjectModels = JsonConvert.DeserializeObject<List<SprintsProjectModel>>(context)!.ToList();
                sprintsProjectModels = sprintsProjectModels.OrderBy(x => x.DateStart).ToList();
                ListSprintsproject.IsVisible = true;
                ListSprintsproject.ItemsSource = sprintsProjectModels.Select(x => new
                {
                    x.Id,
                    Date = $"С {x.DateStart} по {x.DateEnd}",
                    Status = x.TitleStatus
                }).ToList();
            }
        }
    }

    private List<GetSprintModel> getSprintModels = new List<GetSprintModel>();
    private List<SprintsProjectModel> sprintsProjectModels = new List<SprintsProjectModel>();

    private async void Loadsprint(int Id)
    {
        using (var Client = new HttpClient())
        {
            HttpResponseMessage httpResponseMessage = await Client.GetAsync($"{BaseAddress.Address}Sprint/Get_sprint?Email={UserAutorizationTrue.userLog.Email}&Password={UserAutorizationTrue.userLog.Password}&ProjectId={UserAutorizationTrue.ProjectId}&SprintId={Id}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string context = await httpResponseMessage.Content.ReadAsStringAsync();
                getSprintModels = JsonConvert.DeserializeObject<List<GetSprintModel>>(context)!.ToList();
                if (getSprintModels.Count() != 0 && SelectSprint != 0)
                {
                    ListStatusesSprint.IsVisible = true;
                    ListStatusesSprint.ItemsSource = getSprintModels.Select(x => new
                    {
                        x.Id,
                        x.TitleStatus,
                        StatusColorGradient = Avalonia.Media.Color.FromArgb(System.Drawing.ColorTranslator.FromHtml(x.Color).A, System.Drawing.ColorTranslator.FromHtml(x.Color).R, System.Drawing.ColorTranslator.FromHtml(x.Color).G, System.Drawing.ColorTranslator.FromHtml(x.Color).B),
                        ListTasks = x.Tasks!.Select(y => new
                        {
                            IdTask = y.Id,
                            y.TitleTask
                        })
                    }).ToList();
                    IdTimeFromTimers = Id;
                    if (IdTimeFromTimers != 0)
                    {
                        if (sprintsProjectModels.FirstOrDefault(x => x.Id == Id)!.DateStart >= DateTime.Now)
                        {
                            TimerStartInfo();
                        }
                        else if (sprintsProjectModels.FirstOrDefault(x => x.Id == Id)!.DateEnd > DateTime.Now)
                        {
                            TimerEndInfo();
                        }
                        else
                        {
                            _disTimerStart.Stop();
                            _disTimerEnd.Stop();
                            TimerText.Text = "Завершен!";
                        }
                    }
                }
            }
        }
    }

    private int IdTimeFromTimers = 0;
    private DispatcherTimer _disTimerEnd = new DispatcherTimer();
    private DispatcherTimer _disTimerStart = new DispatcherTimer();

    private void UpdateStatusSprint()
    {
        Loadsprints();
    }

    private void TimerEndInfo()
    {
        _disTimerStart.Stop();
        _disTimerEnd.Interval = TimeSpan.FromSeconds(0.1);
        _disTimerEnd.Tick += OnTimedEventEnd;
        _disTimerEnd.Start();
    }
    private void OnTimedEventEnd(object? sender, EventArgs e)
    {
        if (IdTimeFromTimers != 0)
        {
            if (sprintsProjectModels.FirstOrDefault(x => x.Id == IdTimeFromTimers) != null)
            {
                TimeSpan dateTimeEnd = sprintsProjectModels.FirstOrDefault(x => x.Id == IdTimeFromTimers)!.DateEnd.Subtract(DateTime.Now);
                if (dateTimeEnd == new TimeSpan())
                {
                    UpdateStatusSprint();
                }
                TimerText.Text = $"До конца: {dateTimeEnd.Days} дн. {dateTimeEnd.Hours} час. {dateTimeEnd.Minutes} мин. {dateTimeEnd.Seconds} сек.";
            }
            else
            {
                _disTimerEnd.Stop();
                TimerText.Text = "";
            }
        }
        else
        {
            TimerText.Text = "";
        }
    }

    private void TimerStartInfo()
    {
        _disTimerEnd.Stop();
        _disTimerStart.Interval = TimeSpan.FromSeconds(0.1);
        _disTimerStart.Tick += OnTimedEventStart;
        _disTimerStart.Start();
    }
    private void OnTimedEventStart(object? sender, EventArgs e)
    {
        if (IdTimeFromTimers != 0)
        {
            if (sprintsProjectModels.FirstOrDefault(x => x.Id == IdTimeFromTimers) != null)
            {
                TimeSpan dateTimeStart = sprintsProjectModels.FirstOrDefault(x => x.Id == IdTimeFromTimers)!.DateStart.Subtract(DateTime.Now);
                if (dateTimeStart == new TimeSpan())
                {
                    UpdateStatusSprint();
                }
                TimerText.Text = $"До начала: {dateTimeStart.Days} дн. {dateTimeStart.Hours} час. {dateTimeStart.Minutes} мин. {dateTimeStart.Seconds} сек.";
            }
            else
            {
                _disTimerStart.Stop();
                TimerText.Text = "";
            }
        }
        else
        {
            TimerText.Text = "";
        }
    }






    private int IdStatusDrop = 0;

    //Действие тащить
    async void DoDrag(object? sender, PointerPressedEventArgs e)
    {
        if (sprintsProjectModels.FirstOrDefault(x => x.Id == IdTimeFromTimers)!.DateEnd > DateTime.Now &&
            sprintsProjectModels.FirstOrDefault(x => x.Id == IdTimeFromTimers)!.DateStart < DateTime.Now)
        {
            //Результат действия тащить
            var result = await DragDrop.DoDragDrop(e, new DataObject(), DragDropEffects.Move);
            switch (result)
            {
                case DragDropEffects.Move:
                    int Id = (int)(sender as Control).Tag!;
                    using (var Client = new HttpClient())
                    {
                        UpdateStatusTaskModel updateStatusTaskModel = new UpdateStatusTaskModel();
                        updateStatusTaskModel.TaskId = Id;
                        updateStatusTaskModel.StatusId = IdStatusDrop;
                        updateStatusTaskModel.ProjectId = UserAutorizationTrue.ProjectId;
                        updateStatusTaskModel.Password = UserAutorizationTrue.userLog.Password;
                        updateStatusTaskModel.Email = UserAutorizationTrue.userLog.Email;
                        HttpResponseMessage httpResponseMessage = await Client.PutAsJsonAsync($"{BaseAddress.Address}Task/Update_status_task", updateStatusTaskModel);
                        if (httpResponseMessage.IsSuccessStatusCode)
                        {
                            Loadsprint(SelectSprint);
                        }
                    }
                    break;
                case DragDropEffects.None:
                    break;
            }
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
        IdStatusDrop = (int)selectElement.Tag;
        e.DragEffects = DragDropEffects.Move;
    }
















    private async void LoadInfoProject()
    {
        if (UserAutorizationTrue.ProjectId == Guid.Empty)
        {
            ProjectVisible.IsVisible = false;
            MenuButtonsProject.IsVisible = false;
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
        Loadsprint(SelectSprint);
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
        LoadStile();
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
                DounBorder.IsVisible = true;
                SprintsList.IsVisible = false;
                TimerText.IsVisible = false;
                LoadTasks();
                break;
            case 2:
                SprintsButtonMenu.Background = Avalonia.Media.Brushes.LightGray;
                TasksButtonMenu.Background = Avalonia.Media.Brushes.DimGray;
                TaskMenu.IsVisible = false;
                SprintMenu.IsVisible = true;
                ListTasks.IsVisible = false;
                DounBorder.IsVisible = false;
                SprintsList.IsVisible = true;
                Loadsprints();
                TimerText.IsVisible = true;
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

    private int SelectSprint = 0;

    private void SprintLoadButton(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        SelectSprint = (int)(sender as Border).Tag!;
        Loadsprint(SelectSprint);
    }

    private async void NewSprintButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        CreateSprintWindow createSprintWindow = new CreateSprintWindow();
        await createSprintWindow.ShowDialog(this);
        Loadsprints();
    }

    private async void DeleteStatusButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        int id = (int)(sender as MenuItem).Tag!;
        using (var Client = new HttpClient())
        {
            HttpResponseMessage httpResponseMessage = await Client.DeleteAsync($"{BaseAddress.Address}StatusTask/Delete_status_tasks?Email={UserAutorizationTrue.userLog.Email}&Password={UserAutorizationTrue.userLog.Password}&ProjectId={UserAutorizationTrue.ProjectId}&StatusId={id}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                Loadsprint(SelectSprint);
            }
        }
    }

    private async void DeleteSprintButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        int id = (int)(sender as MenuItem).Tag!;
        using (var Client = new HttpClient())
        {
            HttpResponseMessage httpResponseMessage = await Client.DeleteAsync($"{BaseAddress.Address}Sprint/Delete_sprint?Email={UserAutorizationTrue.userLog.Email}&Password={UserAutorizationTrue.userLog.Password}&ProjectId={UserAutorizationTrue.ProjectId}&SprintId={id}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                Loadsprints();
                ListStatusesSprint.IsVisible = false;
            }
        }
    }

    private async void DeleteTaskButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        int id = (int)(sender as MenuItem).Tag!;
        using (var Client = new HttpClient())
        {
            HttpResponseMessage httpResponseMessage = await Client.DeleteAsync($"{BaseAddress.Address}Task/Deleting_task?Email={UserAutorizationTrue.userLog.Email}&Password={UserAutorizationTrue.userLog.Password}&ProjectId={UserAutorizationTrue.ProjectId}&TaskId={id}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                LoadTasks();
            }
        }
    }

    private async void EditSprintButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        int id = (int)(sender as MenuItem).Tag!;
        CreateSprintWindow createSprintWindow = new CreateSprintWindow(id);
        await createSprintWindow.ShowDialog(this);
        Loadsprints();
        Loadsprint(SelectSprint);
    }

    private async void ExitProjectButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        using (var Client = new HttpClient())
        {
            Guid id = (Guid)(sender as MenuItem).Tag!;
            HttpResponseMessage httpResponseMessage = await Client.DeleteAsync($"{BaseAddress.Address}Project/Exit_object_project?Email={UserAutorizationTrue.userLog.Email}&Password={UserAutorizationTrue.userLog.Password}&ProjectId={id}&UserId={UserAutorizationTrue.userLog.Id}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                if (UserAutorizationTrue.ProjectId == id)
                {
                    ProjectVisible.IsVisible = false;
                    MenuButtonsProject.IsVisible = false;
                }
                LoadProjects();
            }
        }
    }

    private async void MyTasksWindowButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        TopButtonVisible.IsVisible = false;
        MenuButtonsProject.IsVisible = false;
        ListTasks.IsVisible = false;
        SprintsList.IsVisible = false;
        ListTasksUserWindow.IsVisible = true;
        PanelProject.DisplayMode = SplitViewDisplayMode.Overlay;
        using (var Client = new HttpClient())
        {
            HttpResponseMessage httpResponseMessage = await Client.GetAsync($"{BaseAddress.Address}Task/Get_tasks_user?Email={UserAutorizationTrue.userLog.Email}&Password={UserAutorizationTrue.userLog.Password}");
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string context = await httpResponseMessage.Content.ReadAsStringAsync();
                List<GetTasksUserModel> tasks = JsonConvert.DeserializeObject<List<GetTasksUserModel>>(context)!.ToList();
                ListTasksUser.ItemsSource = tasks.Select(x => new
                {
                    x.NameProject,
                    ListTasksUserInList = x.ListTasks!.Select(y => new
                    {
                        y.TitleTask, 
                        y.DescriptionTask,
                        ColorTask = new SolidColorBrush(Avalonia.Media.Color.FromArgb(System.Drawing.ColorTranslator.FromHtml(y.Color!).A, System.Drawing.ColorTranslator.FromHtml(y.Color!).R, System.Drawing.ColorTranslator.FromHtml(y.Color!).G, System.Drawing.ColorTranslator.FromHtml(y.Color!).B)),
                        y.TitleStatus
                    }).ToList()
                }).ToList();
            }
        }
    }

    private void DayNightStile()
    {
        if (DarckLight)
        {
            Avalonia.Application.Current!.RequestedThemeVariant = ThemeVariant.Light;
            DarckLight = !DarckLight;
            ButtonIconDayNight.Data = Geometry.Parse("M23.9922 38.4651C24.6394 38.4651 25.1717 38.957 25.2358 39.5873L25.2422 39.7151V42.7527C25.2422 43.443 24.6826 44.0027 23.9922 44.0027C23.345 44.0027 22.8127 43.5108 22.7487 42.8805L22.7422 42.7527V39.7151C22.7422 39.0248 23.3019 38.4651 23.9922 38.4651ZM35.8947 34.0978L35.9962 34.1889L38.1441 36.3368C38.6322 36.8249 38.6322 37.6164 38.1441 38.1046C37.6884 38.5602 36.9686 38.5905 36.4778 38.1957L36.3763 38.1046L34.2284 35.9567C33.7403 35.4685 33.7403 34.6771 34.2284 34.1889C34.684 33.7333 35.4039 33.7029 35.8947 34.0978ZM13.755 34.1889C14.2106 34.6445 14.241 35.3644 13.8461 35.8552L13.755 35.9567L11.6071 38.1046C11.119 38.5927 10.3275 38.5927 9.83937 38.1046C9.38376 37.6489 9.35339 36.9291 9.74825 36.4383L9.83937 36.3368L11.9872 34.1889C12.4754 33.7008 13.2668 33.7008 13.755 34.1889ZM23.9999 13.0805C30.0306 13.0805 34.9194 17.9693 34.9194 24C34.9194 30.0306 30.0306 34.9194 23.9999 34.9194C17.9693 34.9194 13.0805 30.0306 13.0805 24C13.0805 17.9693 17.9693 13.0805 23.9999 13.0805ZM23.9999 15.5805C19.35 15.5805 15.5805 19.3501 15.5805 24C15.5805 28.6499 19.35 32.4194 23.9999 32.4194C28.6499 32.4194 32.4194 28.6499 32.4194 24C32.4194 19.3501 28.6499 15.5805 23.9999 15.5805ZM42.7308 22.787C43.4212 22.787 43.9808 23.3467 43.9808 24.037C43.9808 24.6842 43.489 25.2166 42.8586 25.2806L42.7308 25.287H39.6933C39.0029 25.287 38.4433 24.7274 38.4433 24.037C38.4433 23.3898 38.9352 22.8575 39.5655 22.7935L39.6933 22.787H42.7308ZM8.30657 22.7287C8.99692 22.7287 9.55657 23.2884 9.55657 23.9787C9.55657 24.6259 9.06469 25.1582 8.43437 25.2223L8.30657 25.2287H5.26904C4.57869 25.2287 4.01904 24.6691 4.01904 23.9787C4.01904 23.3315 4.51092 22.7992 5.14124 22.7352L5.26904 22.7287H8.30657ZM11.5056 9.8043L11.6071 9.89542L13.755 12.0433C14.2432 12.5314 14.2432 13.3229 13.755 13.811C13.2994 14.2667 12.5796 14.297 12.0887 13.9022L11.9872 13.811L9.83937 11.6632C9.35122 11.175 9.35122 10.3836 9.83937 9.89542C10.295 9.43981 11.0148 9.40943 11.5056 9.8043ZM38.1441 9.89542C38.5997 10.351 38.63 11.0709 38.2352 11.5617L38.1441 11.6632L35.9962 13.811C35.508 14.2992 34.7166 14.2992 34.2284 13.811C33.7728 13.3554 33.7425 12.6356 34.1373 12.1448L34.2284 12.0433L36.3763 9.89542C36.8644 9.40727 37.6559 9.40727 38.1441 9.89542ZM24.0004 3.99731C24.6476 3.99731 25.18 4.48919 25.244 5.11951L25.2504 5.24731V8.28484C25.2504 8.97519 24.6908 9.53484 24.0004 9.53484C23.3532 9.53484 22.8209 9.04296 22.7569 8.41264L22.7504 8.28484V5.24731C22.7504 4.55696 23.3101 3.99731 24.0004 3.99731Z");
        }
        else
        {
            Avalonia.Application.Current!.RequestedThemeVariant = ThemeVariant.Dark;
            DarckLight = !DarckLight;
            ButtonIconDayNight.Data = Geometry.Parse("M9.66862399,33.0089622 C14.6391867,41.6182294 25.647814,44.5679822 34.2570813,39.5974194 C36.6016136,38.243803 38.5753268,36.4126078 40.0785961,34.229664 C40.5811964,33.4998226 40.256086,32.4918794 39.421758,32.193262 C32.6414364,29.766492 29.0099482,26.9542522 26.9026684,22.9317305 C24.6842213,18.6970048 24.110919,14.0582926 25.662851,7.69987534 C25.8774494,6.82064469 25.1829812,5.98348115 24.27924,6.03196802 C21.4771404,6.18230425 18.739608,6.98721743 16.2570813,8.42050489 C7.64781404,13.3910676 4.69806124,24.3996949 9.66862399,33.0089622 Z M24.6881449,24.0918536 C26.9913881,28.4884439 30.80035,31.5226926 37.1145781,33.998575 C35.9639388,35.3646621 34.5800621,36.524195 33.0070813,37.4323559 C25.5935456,41.7125627 16.1138943,39.1724978 11.8336875,31.7589622 C7.55348069,24.3454265 10.0935456,14.8657752 17.5070813,10.5855684 C19.0445047,9.69793654 20.6992772,9.08707059 22.4136896,8.76727896 L22.882692,8.68729053 C21.6894389,14.6550319 22.2911719,19.5163454 24.6881449,24.0918536 Z");
        }
        /*ThemeVariant A = Avalonia.Application.Current!.ActualThemeVariant;*/
    }

    public static bool DarckLight = true;
    private void DayNightButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        DayNightStile();
    }
}