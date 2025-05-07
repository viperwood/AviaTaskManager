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
using System.Net.Http.Json;
using Metsys.Bson;

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
        UserName.Text = UserAutorizationTrue.userLog.Username + "#" + UserAutorizationTrue.userLog.Id;
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
                UserInfoButton.Background = Brushes.DimGray;
                MailBoxButton.Background = Brushes.DimGray;
                UserFrendsButtonP.Background = Brushes.DimGray;
                UserInfoPanel.IsVisible = false;
                MailBoxPanel.IsVisible = false;
                FrendsPanel.IsVisible = false;
                break;
            case 1:
                Panel.IsPaneOpen = true;
                Panel.OpenPaneLength = 600;
                UserInfoButton.Background = Brushes.LightGray;
                MailBoxButton.Background = Brushes.DimGray;
                UserFrendsButtonP.Background = Brushes.DimGray;
                UserInfoPanel.IsVisible = true;
                MailBoxPanel.IsVisible = false;
                FrendsPanel.IsVisible = false;
                break;
            case 2:
                Panel.IsPaneOpen = true;
                Panel.OpenPaneLength = 800;
                UserInfoButton.Background = Brushes.DimGray;
                MailBoxButton.Background = Brushes.LightGray;
                UserFrendsButtonP.Background = Brushes.DimGray;
                UserInfoPanel.IsVisible = false;
                MailBoxPanel.IsVisible = true;
                FrendsPanel.IsVisible = false;
                break;
            case 3:
                Panel.IsPaneOpen = true;
                Panel.OpenPaneLength = 600;
                UserInfoButton.Background = Brushes.DimGray;
                MailBoxButton.Background = Brushes.DimGray;
                UserFrendsButtonP.Background = Brushes.LightGray;
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
                LoadInfoProject();
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
                    ColorUser = x.AddresseeId == IdFrend ? Brushes.Gray : Brushes.DimGray
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

    private async void EditMailTextButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
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
                    ErrorAddFrend.Foreground = Brushes.Green;
                }
                else
                {
                    ErrorAddFrend.Foreground = Brushes.Red;
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
                ButtonProjectRightPanelInfo.Background = Brushes.DimGray;
                ButtonProjectRightPanelMail.Background = Brushes.DimGray;
                PanelProjectInfo.IsVisible = false;
                PanelProjectMails.IsVisible = false;
                break;
            case 1:
                PanelProject.IsPaneOpen = true;
                PanelProject.OpenPaneLength = 600;
                ButtonProjectRightPanelInfo.Background = Brushes.LightGray;
                ButtonProjectRightPanelMail.Background = Brushes.DimGray;
                PanelProjectInfo.IsVisible = true;
                PanelProjectMails.IsVisible = false;
                break;
            case 2:
                PanelProject.IsPaneOpen = true;
                PanelProject.OpenPaneLength = 800;
                ButtonProjectRightPanelInfo.Background = Brushes.DimGray;
                ButtonProjectRightPanelMail.Background = Brushes.LightGray;
                PanelProjectInfo.IsVisible = false;
                PanelProjectMails.IsVisible = true;
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
            LoadInfoProject();
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
}