using Avalonia;
using Avalonia.Controls;
using System;
using System.Net.Mail;
using System.Net;
using System.Linq;
using TaskMonagerAviaProject.Models;
using System.IO;
using Bitmap = Avalonia.Media.Imaging.Bitmap;
using System.Net.Http;
using System.Net.Http.Json;
using TaskMonagerAviaProject.StaticObjects;
using Brushes = Avalonia.Media.Brushes;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace TaskMonagerAviaProject;

public partial class EditUserLoginWindow : Window
{
    public EditUserLoginWindow()
    {
        InitializeComponent();
        PasswordImageReg.Source = iconsOpen!;
        PasswordImageRegSec.Source = iconsOpen!;
        LoadInfoUser();
    }

    private void LoadInfoUser()
    {
        UserNameTextReg.Text = UserAutorizationTrue.userLog.Username;
        EmailTextReg.Text = UserAutorizationTrue.userLog.Email;
        PasswordTextReg.Text = UserAutorizationTrue.userLog.Password;
        if (UserAutorizationTrue.userLog.UserImage != null)
        {
            using (MemoryStream memoryStream = new MemoryStream(UserAutorizationTrue.userLog.UserImage))
            {
                ImageTest.Source = new Bitmap(memoryStream);
            }
        }
    }

    private bool PasswordBut2 = true;
    private bool PasswordBut3 = true;
    private Bitmap iconsOpen = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + @"\Images\iconsOpen.png");
    private Bitmap iconsClose = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + @"\Images\iconsClose.png");
    private void OpenClosePasswordRegButton(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        PasswordBut2 = !PasswordBut2;
        if (PasswordBut2)
        {
            PasswordImageReg.Source = iconsOpen;
            PasswordTextReg.PasswordChar = '•';
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
            SecondPasswordTextReg.PasswordChar = '•';
        }
        else
        {
            PasswordImageRegSec.Source = iconsClose;
            SecondPasswordTextReg.PasswordChar = '\0';
        }
    }

    private byte[] imageData;

    private void EditUserButtonSave(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (EmailTextReg.Text != UserAutorizationTrue.userLog.Email)
        {
            RegistrationEmail.IsVisible = true;
            Registration.IsVisible = false;
            RegEmail();
        }
        else
        {
            EditSave();
        }
    }

    private async void EditSave()
    {
        if (!string.IsNullOrEmpty(EmailTextReg.Text) && !string.IsNullOrEmpty(PasswordTextReg.Text) && !string.IsNullOrEmpty(UserNameTextReg.Text) && !string.IsNullOrEmpty(SecondPasswordTextReg.Text))
        {
            if (SecondPasswordTextReg.Text == PasswordTextReg.Text)
            {
                using (var Client = new HttpClient())
                {
                    EditUserModel registrationModel = new EditUserModel();
                    registrationModel.EmailNew = EmailTextReg.Text;
                    registrationModel.EmailOld = UserAutorizationTrue.userLog.Email!;
                    registrationModel.PasswordUser = PasswordTextReg.Text;
                    registrationModel.PasswordUserOld = UserAutorizationTrue.userLog.Password!;
                    registrationModel.Username = UserNameTextReg.Text;
                    registrationModel.ImageUser = imageData;
                    HttpResponseMessage httpResponseMessage = await Client.PutAsJsonAsync($"{BaseAddress.Address}User/Edit_user", registrationModel);
                    if (httpResponseMessage.IsSuccessStatusCode)
                    {
                        ErrorTextRegEmail.Foreground = Brushes.Green;
                        ErrorTextRegEmail.Text = await httpResponseMessage.Content.ReadAsStringAsync();
                        ErrorTextReg.Foreground = Brushes.Green;
                        ErrorTextReg.Text = await httpResponseMessage.Content.ReadAsStringAsync();
                        await Task.Delay(1500);
                        LoginUserModel loginUserModel = new LoginUserModel();
                        loginUserModel.Email = EmailTextReg.Text;
                        loginUserModel.PasswordUser = PasswordTextReg.Text;
                        httpResponseMessage = await Client.PostAsJsonAsync($"{BaseAddress.Address}User/Login_user", loginUserModel);
                        if (httpResponseMessage.IsSuccessStatusCode)
                        {
                            string path = AppDomain.CurrentDomain.BaseDirectory + @"\UserLog.json";
                            string content = await httpResponseMessage.Content.ReadAsStringAsync();
                            UserAutorizationTrue.userLog = JsonConvert.DeserializeObject<List<User>>(content)![0];
                            if (File.Exists(path))
                            {
                                if (!string.IsNullOrEmpty(content) && !string.IsNullOrEmpty(path))
                                {
                                    File.WriteAllText(path, content);
                                }
                            }
                        }
                        Close();
                    }
                    else
                    {
                        ErrorTextRegEmail.Foreground = Brushes.Red;
                        ErrorTextRegEmail.Text = await httpResponseMessage.Content.ReadAsStringAsync();
                        ErrorTextReg.Foreground = Brushes.Red;
                        ErrorTextReg.Text = await httpResponseMessage.Content.ReadAsStringAsync();
                        await Task.Delay(1500);

                    }
                }
            }
        }
    }

    public static byte[] ConvertBitmapToByteArray(System.Drawing.Bitmap bitmap)
    {
        if (bitmap != null)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Сохраняем Bitmap в MemoryStream в формате PNG
                bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                // Возвращаем массив байтов
                return memoryStream.ToArray();
            }
        }
        else
        {
            throw new ArgumentNullException();
        }
    }

    private async void OpenImage(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        TopLevel top = TopLevel.GetTopLevel(this)!;
        var file = await top.StorageProvider.OpenFilePickerAsync(new Avalonia.Platform.Storage.FilePickerOpenOptions
        {
            Title = "Image user"
        });
        if (file.Count() != 0)
        {
            try
            {
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(file[0].Path.AbsolutePath);
                imageData = ConvertBitmapToByteArray(bitmap);
                using (MemoryStream memoryStream = new MemoryStream(imageData))
                {
                    ImageTest.Source = new Bitmap(memoryStream);
                }
            }
            catch
            {

            }
        }
    }

    private int randomKode = 0;

    private async void RegEmail()
    {

        randomKode = new Random().Next(100000000, 999999999);

        MailAddress from = new MailAddress("aviataskmonager@gmail.com", "Avia task monager");
        // кому отправляем
        MailAddress to = new MailAddress($"{EmailTextReg.Text}");
        // создаем объект сообщения
        MailMessage m = new MailMessage(from, to);
        // тема письма
        m.Subject = "Подтверждение почты";
        // текст письма
        m.Body = $"<h2>Ваш код подтверждения: {randomKode}</h2>";
        // письмо представляет код html
        m.IsBodyHtml = true;
        // адрес smtp-сервера и порт, с которого будем отправлять письмо
        SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
        // логин и пароль
        smtp.Credentials = new NetworkCredential("aviataskmonager@gmail.com", "qvcq fjdl nibv anqj");
        smtp.EnableSsl = true;
        await smtp.SendMailAsync(m);
    }

    private void EditUserEmailButtonSave(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (NumberFromEmail.Text == randomKode.ToString())
        {
            EditSave();
        }
    }
}

