using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using TaskMonagerAviaProject.Models;
using TaskMonagerAviaProject.StaticObjects;

namespace TaskMonagerAviaProject
{
    public partial class MainWindow : Window
    {

        private string path = AppDomain.CurrentDomain.BaseDirectory + @"UserLog.json";
        private LoginUserModel loginUserModel = new LoginUserModel();
        private Bitmap iconsOpen = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + @"\Images\iconsOpen.png");
        private Bitmap iconsClose = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + @"\Images\iconsClose.png");

        public MainWindow()
        {
            InitializeComponent();
            PasswordImage.Source = iconsOpen;
            PasswordImageReg.Source = iconsOpen;
            PasswordImageRegSec.Source = iconsOpen;
            if (File.Exists(path))
            {

                User data = JsonConvert.DeserializeObject<List<User>>(File.ReadAllText(path))![0];
                loginUserModel.Email = data.Email!;
                loginUserModel.PasswordUser = data.Password!;
                Autorisation(loginUserModel);
            }
        }

        private void LoginButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(EmailText.Text) && !string.IsNullOrEmpty(PasswordText.Text))
            {
                loginUserModel.Email = EmailText.Text;
                loginUserModel.PasswordUser = PasswordText.Text;
                Autorisation(loginUserModel);
            }
        }

        private async void Autorisation(LoginUserModel loginUserModel)
        {
            using (var Client = new HttpClient())
            {
                HttpResponseMessage httpResponseMessage = await Client.PostAsJsonAsync($"{BaseAddress.Address}User/Login_user", loginUserModel);
                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    string content = await httpResponseMessage.Content.ReadAsStringAsync();
                    UserAutorizationTrue.userLog = JsonConvert.DeserializeObject<List<User>>(content)![0];
                    OpenWindow(content);
                }
                else
                {
                    ErrorText.IsVisible = true;
                    ErrorText.Text = await httpResponseMessage.Content.ReadAsStringAsync();
                }
            }
        }

        private async void Register()
        {
            using (var Client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(EmailTextReg.Text) &&
                    !string.IsNullOrEmpty(PasswordTextReg.Text) &&
                    !string.IsNullOrEmpty(UserNameTextReg.Text) &&
                    !string.IsNullOrEmpty(SecondPasswordTextReg.Text))
                {
                    if (SecondPasswordTextReg.Text == PasswordTextReg.Text)
                    {
                        RegistrationModel registrationModel = new RegistrationModel();
                        registrationModel.Email = EmailTextReg.Text;
                        registrationModel.PasswordUser = PasswordTextReg.Text;
                        registrationModel.Username = UserNameTextReg.Text;
                        HttpResponseMessage httpResponseMessage = await Client.PostAsJsonAsync($"{BaseAddress.Address}User/Registration_user", registrationModel);
                        if (httpResponseMessage.IsSuccessStatusCode)
                        {
                            ErrorTextReg.Foreground = Brushes.Green;
                            ErrorTextReg.Text = await httpResponseMessage.Content.ReadAsStringAsync();
                        }
                        else
                        {
                            ErrorTextReg.Text = await httpResponseMessage.Content.ReadAsStringAsync();
                        }
                    }
                    else
                    {
                        ErrorTextReg.Text = "ѕароли не совпадают!";
                    }
                }
                else
                {
                    ErrorTextReg.Text = "¬се пол€ должны быть заполнены!";
                }
            }
        }

        private void RegWindow()
        {
            Autorise.IsVisible = Reg;
            Reg = !Reg;
            Registration.IsVisible = Reg;
        }

        private void OpenWindow(string content)
        {
            WindowProjects windowProjects = new WindowProjects(content);
            windowProjects.Show();
            Close();
        }

        private bool Reg = false;
        private bool PasswordBut1 = true;
        private bool PasswordBut2 = true;
        private bool PasswordBut3 = true;

        private void RegistrationButton(object? sender, Avalonia.Input.TappedEventArgs e)
        {
            RegWindow();
        }

        private void RegButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Register();
        }

        private void Back(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            RegWindow();
        }

        private void OpenClosePasswordButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            PasswordBut1 = !PasswordBut1;
            if (PasswordBut1)
            {
                PasswordImage.Source = iconsOpen;
                PasswordText.PasswordChar = 'Х';
            }
            else
            {
                PasswordImage.Source = iconsClose;
                PasswordText.PasswordChar = '\0';
            }
        }

        private void OpenClosePasswordRegButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            PasswordBut2 = !PasswordBut2;
            if (PasswordBut2)
            {
                PasswordImageReg.Source = iconsOpen;
                PasswordTextReg.PasswordChar = 'Х';
            }
            else
            {
                PasswordImageReg.Source = iconsClose;
                PasswordTextReg.PasswordChar = '\0';
            }
        }

        private void OpenClosePasswordRegSecondButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            PasswordBut3 = !PasswordBut3;
            if (PasswordBut3)
            {
                PasswordImageRegSec.Source = iconsOpen;
                SecondPasswordTextReg.PasswordChar = 'Х';
            }
            else
            {
                PasswordImageRegSec.Source = iconsClose;
                SecondPasswordTextReg.PasswordChar = '\0';
            }
        }
    }
}