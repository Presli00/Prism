using PrismTest.Models;
using PrismTest.Models.DataModels;
using PrismTest.Properties;
using PrismTest.ViewModels;
using System;
using System.Diagnostics;
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
                foreach (var item in context.Games)
                {
                    ContentPresenter c = (ContentPresenter)storeListView.ItemContainerGenerator.ContainerFromItem(item);
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
        }

        private void GamePage_OnClick(object sender, RoutedEventArgs e)
        {
            object link = ((Button)sender).Tag;
            string linkString = link.ToString().Trim();

            if (linkString != string.Empty)
            {
                Process.Start(new ProcessStartInfo(linkString));
            }
        }

        private void BuyGame_OnClick(object sender, RoutedEventArgs e)
        {
            object link = ((Button)sender).Tag;
            string linkstring = link.ToString().Trim();

            if (linkstring != string.Empty)
            {
                Process.Start(new ProcessStartInfo(linkstring));
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
