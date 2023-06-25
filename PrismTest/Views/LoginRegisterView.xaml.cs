using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using PrismTest.Models.DataModels;
using PrismTest.ViewModels;
using PrismTest.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;

namespace PrismTest.Views
{
    public partial class LoginRegisterView : UserControl
    {
        //private const int keySize = 64;
        //private const int iterations = 350000;
        //HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;

        private readonly ModelContext context = new ModelContext();
        private MainWindow mainWindow = ((MainWindow)Application.Current.MainWindow);
        private PasswordsEncryption encryptor = new PasswordsEncryption();
        public string allusernames;

        public LoginRegisterView()
        {
            InitializeComponent();
        }

        private void Register_OnClick(object sender, RoutedEventArgs e)
        {
            string username = Username.Text;
            string email = RegisterEmail.Text;
            string password = RegisterPassword.Password;
            string confirmPassword = ConfirmPassword.Password;
            string region = Region.Text;

            if (string.IsNullOrEmpty(username))
            {
                errorMessage.Text = "Please enter an username.";
            }
            else if (!Regex.IsMatch(username, @"^(?=.{8,20}$)(?![_.])(?!.*[_.]{2})[a-zA-Z0-9._]+(?<![_.])$"))
            {
                errorMessage.Text = "Plese enter a valid username.";
            }
            else if (this.context.Users.Any(x => x.Username == username))
            {
                errorMessage.Text = "A user with this username already exists";
            }
            else if (string.IsNullOrEmpty(email))
            {
                errorMessage.Text = "Please enter an email.";
            }
            else if (!Regex.IsMatch(email, @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$"))
            {
                errorMessage.Text = "Plese enter a valid email.";
            }
            else if (this.context.Users.Any(x => x.Email == email))
            {
                errorMessage.Text = "A user with this email already exists";
            }
            else if (string.IsNullOrEmpty(password))
            {
                errorMessage.Text = "Plese enter a password.";
            }

            else if (!Regex.IsMatch(password, @"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$"))
            {
                errorMessage.Text = "Plese enter a valid password. Password must contain:" +
                    "\n At least one upper case English letter" +
                    "\n At least one lower case English letter" +
                    "\n At least one digit" +
                    "\n At least one special character" +
                    "\n Minimum eight in length";
            }
            else if (string.IsNullOrEmpty(confirmPassword))
            {
                errorMessage.Text = "Plese confirm password.";
            }
            else if (confirmPassword != password)
            {
                errorMessage.Text = "You must enter the same password";
            }
            else
            {
                var encrypted = encryptor.HashPassword(password, out var salt);
                var hex = Convert.ToHexString(salt);
                this.context.Users.Add(new User
                {
                    Username = username,
                    Password = encrypted,
                    Email = email,
                    Salt = hex,
                    Region = region,
                    WalletBalance = 0,
                    Purchases = null
                });

                this.context.SaveChanges();
                this.BackupUser(username, email, encrypted, hex, region, 0);

                mainWindow.AddGameButton.Visibility = Visibility.Visible;
                mainWindow.Menu.Visibility = Visibility.Visible;
                mainWindow.ContentControl.Margin = new Thickness(0, 64, 0, 0);
                mainWindow.LoadAllViews();
                mainWindow.Username.Text = username;
                mainWindow.Email.Text = email;
                mainWindow.Wallet.Text = 0 + "€";
                LoadAllGames lag = new LoadAllGames();
                lag.LoadGames(email);
                context.Dispose();
            }
        }

        private void BackupUser(string username, string email, string password, string salt, string region, int ballance)
        {
            if (System.IO.File.Exists($"./Resources/Users.txt"))
            {
                string[] allusers = System.IO.File.ReadAllLines($"./Resources/Users.txt");
                string[] columns = new string[0];
                int numofusers = 0;
                foreach (var item in allusers)
                {
                    columns = allusers[numofusers].Split('|');
                    string user = columns[0];
                    user = columns[0];
                    user = user.Trim().ToLower();
                    allusernames = allusernames + " | " + user + " | ";
                    numofusers++;
                }
                if (!allusernames.Contains(" | " + username.Trim().ToLower() + " | "))
                {
                    try
                    {
                        TextWriter tsw = new StreamWriter($@"./Resources/Users.txt", true);
                        Guid userGuid = Guid.NewGuid();
                        tsw.WriteLine(username + "|" +
                                          email + "|" +
                                          password + "|" +
                                          salt + "|" +
                                          region + "|" +
                                          ballance + "|" +
                                      userGuid);
                        tsw.Close();
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(DateTime.Now + ": user backed up: " + ex.Message);
                    }
                    Trace.WriteLine(DateTime.Now + ": Added Game automatically: " + username);
                }
            }
            else
            {
                try
                {
                    TextWriter tsw = new StreamWriter($@"./Resources/Users.txt", true);
                    Guid userGuid = Guid.NewGuid();
                    tsw.WriteLine(username + "|" +
                                          email + "|" +
                                          password + "|" +
                                          salt + "|" +
                                          region + "|" +
                                          ballance + "|" +
                                      userGuid);
                    tsw.Close();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(DateTime.Now + ": user backed up2: " + ex.Message);
                }
            }
        }

        private void Login_OnClick(object sender, RoutedEventArgs e)
        {
            string email = LoginEmail.Text;
            string password = LoginPassword.Password;

            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                var user = this.context.Users.FirstOrDefault(x => x.Email.Equals(email));

                if (user != null)
                {
                    if (!encryptor.VerifyPassword(password, user.Password, Convert.FromHexString(user.Salt)))
                    {
                        errorMessage.Text = "The password is wrong.";
                    }
                    else
                    {
                        mainWindow.AddGameButton.Visibility = Visibility.Visible;
                        mainWindow.Menu.Visibility = Visibility.Visible;
                        mainWindow.ContentControl.Margin = new Thickness(0, 64, 0, 0);
                        mainWindow.Username.Text = user.Username;
                        mainWindow.Email.Text = user.Email;
                        mainWindow.Wallet.Text = user.WalletBalance + "€";
                        mainWindow.LoadAllViews();
                        context.Dispose();
                    }
                }
                else if (File.Exists($"./Resources/Users.txt"))
                {
                    string userFile = $"./Resources/Users.txt";
                    string[] columns = new string[0];
                    foreach (var item in File.ReadAllLines(userFile))
                    {
                        columns = item.Split('|');
                        if (columns[1] == email)
                        {
                            if (!encryptor.VerifyPassword(password, user.Password, Convert.FromHexString(user.Salt)))
                            {
                                errorMessage.Text = "The password is wrong.";
                            }
                            else
                            {
                                mainWindow.AddGameButton.Visibility = Visibility.Visible;
                                mainWindow.Menu.Visibility = Visibility.Visible;
                                mainWindow.ContentControl.Margin = new Thickness(0, 64, 0, 0);
                                mainWindow.Username.Text = columns[0];
                                mainWindow.Email.Text = columns[1];
                                mainWindow.Wallet.Text = columns[5] + "€";
                                mainWindow.LoadAllViews();
                            }
                        }
                    }
                }
                else
                {
                    errorMessage.Text = "There is no user with this email.";
                }
            }
            else
            {
                errorMessage.Text = "Please enter a username or password.";
            }
        }
    }
}