using PrismTest.Models;
using PrismTest.Models.DataModels;
using PrismTest.Properties;
using PrismTest.ViewModels;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace PrismTest.Views
{
    /// <summary>
    /// Interaction logic for StoreView.xaml
    /// </summary>
    public partial class StoreView : UserControl
    {
        private readonly ModelContext context = new ModelContext();
        DateTime dt;
        DispatcherTimer t;
        public DoubleAnimation doubleAnimation = new DoubleAnimation();
        public static string FilterGenreName;
        private MainWindow MainWindow = ((MainWindow)Application.Current.MainWindow);
        public CollectionViewSource StoreListCVS;
        public string alltitles;

        public StoreView()
        {
            InitializeComponent();
            t = new DispatcherTimer();
            t.Tick += new EventHandler(t_Tick);
        }

        private void Marquee_Start(object sender, MouseEventArgs e)
        {
            dt = DateTime.Now;
            t.Interval = new TimeSpan(0, 0, 1);
            t.IsEnabled = true;
        }

        private void Marquee_Stop(object sender, MouseEventArgs e)
        {
            t.IsEnabled = false;

            if (storeListView.Items.Count != 0)
            {
                foreach (var item in storeListView.Items)
                {
                    ContentPresenter c = (ContentPresenter)storeListView.ItemContainerGenerator.ContainerFromItem(item);
                    TextBlock title = c.ContentTemplate.FindName("StoreGameTitle", c) as TextBlock;
                    Canvas canvas = c.ContentTemplate.FindName("canvasTitle", c) as Canvas;
                    MaterialDesignThemes.Wpf.Card card = c.ContentTemplate.FindName("gameCard", c) as MaterialDesignThemes.Wpf.Card;
                    if (card.IsMouseOver != true)
                    {
                        if (title.Text.ToString().Length > 20)
                        {
                            title.BeginAnimation(Canvas.RightProperty, null);
                        }
                    }
                }
            }
        }
        public void t_Tick(object sender, EventArgs e)
        {
            if ((DateTime.Now - dt).Seconds >= 2)
            {
                if (storeListView.Items.Count != 0)
                {
                    foreach (var item in storeListView.Items)
                    {
                        ContentPresenter c = (ContentPresenter)storeListView.ItemContainerGenerator.ContainerFromItem(item);
                        TextBlock title = c.ContentTemplate.FindName("StoreGameTitle", c) as TextBlock;
                        Canvas canvas = c.ContentTemplate.FindName("canvasTitle", c) as Canvas;
                        MaterialDesignThemes.Wpf.Card card = c.ContentTemplate.FindName("gameCard", c) as MaterialDesignThemes.Wpf.Card;
                        if (card.IsMouseOver == true)
                        {
                            if (title.Text.ToString().Length > 22)
                            {
                                doubleAnimation.From = 0;
                                doubleAnimation.To = canvas.ActualWidth;
                                doubleAnimation.RepeatBehavior = RepeatBehavior.Forever;
                                doubleAnimation.Duration = new Duration(TimeSpan.Parse("0:0:5"));
                                title.BeginAnimation(Canvas.RightProperty, doubleAnimation);
                            }
                        }
                    }
                }
            }
        }

        private void UpdateColours(object sender, RoutedEventArgs e)
        {
            var converter = new BrushConverter();

            if (context.Games.Count() != 0)
            {
                for (int i = 0; i < storeListView.Items.Count; i++)
                {

                    ContentPresenter c = (ContentPresenter)storeListView.ItemContainerGenerator.ContainerFromItem(storeListView.Items[i]);
                    try
                    {
                        Button tb = c.ContentTemplate.FindName("GameTitleBtn", c) as Button;
                        tb.Foreground = (Brush)converter.ConvertFromString(Settings.Default.gametitles);
                    }
                    catch (Exception br)
                    {
                        Trace.WriteLine("Break: " + br);
                        break;
                    }
                }
            }
            else if (storeListView.Items.Count != 0)
            {
                for (int i = 0; i < storeListView.Items.Count; i++)
                {
                    storeListView.UpdateLayout();
                    ContentPresenter c = (ContentPresenter)storeListView.ItemContainerGenerator.ContainerFromItem(storeListView.Items[i]);
                    try
                    {
                        Button tb = c.ContentTemplate.FindName("GameTitleBtn", c) as Button;
                        tb.Foreground = (Brush)converter.ConvertFromString(Settings.Default.gametitles);
                    }
                    catch (Exception br) { Trace.WriteLine("Break: " + br); break; }
                }
            }
        }

        private void GamePage_OnClick(object sender, RoutedEventArgs e)
        {
            object link = ((Button)sender).Tag;
            string linkString = link.ToString().Trim();

            if (!string.IsNullOrEmpty(linkString) || linkString != "")
            {
                Process.Start(new ProcessStartInfo(linkString));
            }
        }

        private void BuyGame_OnClick(object sender, RoutedEventArgs e)
        {
            object link = ((Button)sender).Tag;
            string linkstring = link.ToString().Trim();

            if (!string.IsNullOrEmpty(linkstring) || linkstring != "")
            {
                Process.Start(new ProcessStartInfo(linkstring));
            }
        }

        private void BuyDirectly_OnClick(object sender, RoutedEventArgs e)
        {
            object link = ((Button)sender).Tag;
            string linkString = link.ToString().Trim();

            if (!string.IsNullOrEmpty(linkString))
            {
                var game = context.Games.FirstOrDefault(x => x.Title == linkString);
                var user = context.Users.FirstOrDefault(x => x.Email == MainWindow.Email.Text);
                var purchase = context.PurchaseHistory.FirstOrDefault(x => x.Game == game);

                if (purchase != null)
                {
                    if (user.WalletBalance >= game.Price)
                    {
                        purchase.PurchaseAmount++;
                        user.WalletBalance = user.WalletBalance - game.Price;
                        MainWindow.Wallet.Text = user.WalletBalance.ToString();
                    }
                    else
                    {
                        Trace.WriteLine(DateTime.Now + "Not enough money in the wallet");
                    }
                }
                else
                {
                    if (user.WalletBalance >= game.Price)
                    {
                        context.Purchase.Add(new PurchaseHistory
                        {
                            UserId = user.Id,
                            User = user,
                            GameId = game.Id,
                            Game = game,
                            PurchaseDate = DateTime.Now,
                            PurchaseAmount = 1
                        });
                        user.WalletBalance = user.WalletBalance - game.Price;
                        MainWindow.Wallet.Text = user.WalletBalance.ToString();
                        if (System.IO.File.Exists($"./Resources/{MainWindow.Email.Text}GamesList.txt"))
                        {
                            string[] allgames = System.IO.File.ReadAllLines($"./Resources/{MainWindow.Email.Text}GamesList.txt");
                            string[] columns = new string[0];
                            int numofgames = 0;
                            foreach (var item in allgames)
                            {
                                columns = allgames[numofgames].Split('|');
                                string gametitle = columns[0];
                                gametitle = columns[0];
                                gametitle = gametitle.Trim().ToLower();
                                alltitles = alltitles + " | " + gametitle + " | ";
                                numofgames++;
                            }
                            if (!alltitles.Contains(" | " + game.Title.Trim().ToLower() + " | "))
                            {
                                try
                                {
                                    TextWriter tsw = new StreamWriter($@"./Resources/{MainWindow.Email.Text}GamesList.txt", true);
                                    Guid gameGuid = Guid.NewGuid();
                                    tsw.WriteLine(game.Title + "|" +
                                                  game.Genre + "|" +
                                                  "" + "|" +
                                                  game.GamePage + "|" +
                                                  "" + "|" +
                                                  game.Poster + "|" +
                                                  "" + "|" +
                                                  gameGuid);
                                    tsw.Close();
                                }
                                catch (Exception ex)
                                {
                                    Trace.WriteLine(DateTime.Now + ": AddGameOnClick: " + ex.Message);
                                }
                                Trace.WriteLine(DateTime.Now + ": Added Game manually: " + game.Title);
                                ((MainWindow)Application.Current.MainWindow)?.RefreshGames();
                                //((MainWindow)Application.Current.MainWindow).isDialogOpen = false;
                            }
                        }
                    }
                    else
                    {
                        Trace.WriteLine(DateTime.Now + "Not enough money in the wallet");
                    }
                }
                context.SaveChanges();
            }
        }

        private void EnableFilteringCheat(object sender, RoutedEventArgs e)
        {
            StoreListCVS = ((CollectionViewSource)(FindResource("StoreListCVS")));
            MainWindow.cvs = StoreListCVS;
            MainWindow.MenuToggleButton.IsChecked = true;
        }

        private void GameSearch(object sender, FilterEventArgs e)
        {
            GameList gl = e.Item as GameList;
            e.Accepted &= gl.Title.ToUpper().Contains(GameSearchBar.Text.ToUpper());
        }

        private void SearchString_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshList();
        }

        private void GenreFilter(object sender, FilterEventArgs e)
        {
            GameList gl = e.Item as GameList;
            e.Accepted &= gl.Genre.ToUpper().Contains(FilterGenreName.ToUpper());
        }

        private void RefreshList()
        {
            StoreListCVS = ((CollectionViewSource)(FindResource("StoreListCVS")));
            MainWindow.cvs = StoreListCVS;

            if (FilterGenreName != null)
            {
                StoreListCVS.Filter += new FilterEventHandler(GenreFilter);
            }

            if (GameSearchBar.Text != null)
            {
                StoreListCVS.Filter += new FilterEventHandler(GameSearch);
            }

            if (StoreListCVS.View != null)
            {
                StoreListCVS.View.Refresh();
            }
        }
        public void GenreToFilter(string filtergenrename)
        {
            FilterGenreName = filtergenrename;
        }
        public void RefreshList2(CollectionViewSource cvscvs)
        {
            if (cvscvs != null)
            {
                StoreListCVS = cvscvs;
                if (FilterGenreName != null || FilterGenreName != "")
                {
                    StoreListCVS.Filter += new FilterEventHandler(GenreFilter);
                }
                if (GameSearchBar.Text != null)
                {
                    StoreListCVS.Filter += new FilterEventHandler(GameSearch);
                }
                if (StoreListCVS.View != null)
                    StoreListCVS.View.Refresh();
            }
        }
    }
}
