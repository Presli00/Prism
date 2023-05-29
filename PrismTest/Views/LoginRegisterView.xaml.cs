using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using PrismTest.Models.DataModels;
using PrismTest.ViewModels;
using PrismTest.Helpers;
using Microsoft.EntityFrameworkCore;

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
                this.context.Users.Add(new User
                {
                    Username = username,
                    Password = encryptor.HashPassword(password, out var salt),
                    Email = email,
                    Salt = Convert.ToHexString(salt),
                    Region = region,
                    WalletBalance = 0,
                    Purchases = null
                });

                this.context.SaveChanges();

                mainWindow.AddGameButton.Visibility = Visibility.Visible;
                mainWindow.Menu.Visibility = Visibility.Visible;
                mainWindow.ContentControl.Margin = new Thickness(0, 64, 0, 0);
                mainWindow.LoadAllViews();
                mainWindow.Username.Text = username;
                mainWindow.Email.Text = email;
                LoadAllGames lag = new LoadAllGames();
                lag.LoadGames(email);
                context.Dispose();
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
                    if (!encryptor.VerifyPassowrd(password, user.Password, Convert.FromHexString(user.Salt)))
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
                        mainWindow.LoadAllViews();
                        context.Dispose();
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