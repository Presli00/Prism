using PrismTest.Models;
using PrismTest.Models.DataModels;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace PrismTest.ViewModels
{
    public class LoadAllGames
    {
        public ObservableCollection<GameList> StoreGames
        {
            get; set;
        }
        public ObservableCollection<GameList> Games
        {
            get; set;
        }
        public ObservableCollection<GenreList> Genres
        {
            get; set;
        }
        private ObservableCollection<GameList> storeGames = new ObservableCollection<GameList>();
        private ObservableCollection<GameList> games = new ObservableCollection<GameList>();
        private ObservableCollection<GenreList> genres = new ObservableCollection<GenreList>();
        private readonly ModelContext context = new ModelContext();
        public BitmapImage icon;
        public BitmapImage poster;
        public BitmapImage banner;
        public string title;
        public string path;
        public string genre;
        public string link;
        public string guid;
        public string genreName;
        public string genreGuid;
        public int percentage;
        public BitmapImage storePoster;
        public string storeTitle;
        public string gamePage;
        public string storeGenre;
        public string storeLink;
        public string storeGuid;

        public void LoadStoreGames()
        {
            try
            {
                storeGames.Clear();
            }
            catch { }
            try
            {
                StoreGames.Clear();
            }
            catch { }
            LoadStoreList();
            StoreGames = storeGames;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            ((MainWindow)Application.Current.MainWindow).RefreshDataContext()));
        }

        public void LoadGames(string email)
        {
            try
            {
                games.Clear();
            }
            catch { }
            try
            {
                Games.Clear();
            }
            catch { }
            ReadFile(email);
            Games = games;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            ((MainWindow)Application.Current.MainWindow).RefreshDataContext()));
        }

        public void LoadGenres()
        {
            try
            {
                Genres.Clear();
            }
            catch { }
            try
            {
                genres.Clear();
            }
            catch { }
            if (File.Exists("./Resources/GenreList.txt"))
            {
                string genreFile = "./Resources/GenreList.txt";
                string[] genresArr = File.ReadAllLines(genreFile);
                string[] columns = new string[0];
                int numberOfGenres = 0;
                foreach (var item in genresArr)
                {
                    columns = genresArr[numberOfGenres].Split('|');
                    genreName = columns[0];
                    genreGuid = columns[1];
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    AddGenresToOC()));
                    genreName = null;
                    genreGuid = null;
                    numberOfGenres++;
                }
            }
            Genres = genres;
        }

        public void LoadStoreList()
        {
            if (context.Games.Any())
            {
                var games = context.Games.ToList();
                int itemcount = 0;
                int GameCount = games.Count();
                foreach (var item in games)
                {
                    if (item.Poster != "")
                    {
                        try
                        {
                            string installDir = AppDomain.CurrentDomain.BaseDirectory;
                            string posterpath = installDir + "Resources/img/" + item.Poster;
                            BitmapImage image = new BitmapImage();
                            image.BeginInit();
                            image.UriSource = new Uri(posterpath);
                            image.DecodePixelWidth = 200;
                            image.CacheOption = BitmapCacheOption.OnLoad;
                            image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                            image.EndInit();
                            image.Freeze();
                            Dispatcher.CurrentDispatcher.Invoke(() => storePoster = image);
                        }
                        catch (Exception e) { Trace.WriteLine("Error saving image (Poster): " + e); }
                    }
                    itemcount++;
                    storeTitle = item.Title;
                    storeGenre = item.Genre;
                    gamePage = item.GamePage;
                    storeLink = item.BuyLink;
                    double percent = itemcount / GameCount;
                    percentage = Convert.ToInt32(percent);
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    AddStoreGameToOC()));
                    storePoster = null;
                }
            }
            else if (File.Exists($"./Resources/StoreList.txt"))
            {
                string gameFile = $"./Resources/StoreList.txt";
                string[] columns = new string[0];
                int itemcount = 0;
                int GameCount = File.ReadLines(gameFile).Count();
                foreach (var item in File.ReadAllLines(gameFile))
                {
                    columns = item.Split('|');
                    if (columns[4] != "")
                    {
                        try
                        {
                            string installDir = AppDomain.CurrentDomain.BaseDirectory;
                            string posterpath = installDir + "Resources/img/" + columns[5];
                            BitmapImage image = new BitmapImage();
                            image.BeginInit();
                            image.UriSource = new Uri(posterpath);
                            image.DecodePixelWidth = 200;
                            image.CacheOption = BitmapCacheOption.OnLoad;
                            image.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                            image.EndInit();
                            image.Freeze();
                            Dispatcher.CurrentDispatcher.Invoke(() => storePoster = image);
                        }
                        catch (Exception e) { Trace.WriteLine("Error saving image (Poster): " + e); }
                    }
                    itemcount++;
                    storeTitle = columns[0];
                    storeGenre = columns[1];
                    gamePage = columns[2];
                    storeLink = columns[3];
                    storeGuid = columns[5];
                    double percent = itemcount / GameCount;
                    percentage = Convert.ToInt32(percent);
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    AddStoreGameToOC()));
                    storePoster = null;
                }
            }
        }

        public void ReadFile(string email)
        {
            if (context.Purchase.Any(x => x.User.Email == email))
            {
                string gameFile = $"./Resources/{email}GamesList.txt";
                var games = context.Purchase.ToList();
                string[] columns = new string[0];
                int itemcount = 0;
                int GameCount = games.Count();
                foreach (var item in games)
                {
                    var line = File.ReadAllLines(gameFile).FirstOrDefault(x => x.Contains(item.Game.Title));
                    columns = line.Split('|');
                    if (columns[4] != "")
                    {
                        try
                        {
                            string installDir = AppDomain.CurrentDomain.BaseDirectory;
                            string iconpath = installDir + "Resources/img/" + columns[4];
                            icon = new BitmapImage();
                            icon.BeginInit();
                            icon.UriSource = new Uri(iconpath);
                            icon.DecodePixelWidth = 80;
                            icon.CacheOption = BitmapCacheOption.OnLoad;
                            icon.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                            icon.EndInit();
                            icon.Freeze();
                        }
                        catch (Exception e) { Trace.WriteLine("Error saving image (Icon): " + e); }
                    }
                    if (columns[5] != "")
                    {
                        try
                        {
                            string installDir = AppDomain.CurrentDomain.BaseDirectory;
                            string posterpath = installDir + "Resources/img/" + columns[5];
                            poster = new BitmapImage();
                            poster.BeginInit();
                            poster.UriSource = new Uri(posterpath);
                            poster.DecodePixelWidth = 200;
                            poster.CacheOption = BitmapCacheOption.OnLoad;
                            poster.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                            poster.EndInit();
                            poster.Freeze();
                        }
                        catch (Exception e) { Trace.WriteLine("Error saving image (Poster): " + e); }
                    }
                    if (columns[6] != "")
                    {
                        try
                        {
                            string installDir = AppDomain.CurrentDomain.BaseDirectory;
                            string columnpath = installDir + "Resources/img/" + columns[6];
                            banner = new BitmapImage();
                            banner.BeginInit();
                            banner.UriSource = new Uri(columnpath);
                            banner.DecodePixelWidth = 300;
                            banner.CacheOption = BitmapCacheOption.OnLoad;
                            banner.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                            banner.EndInit();
                            banner.Freeze();
                        }
                        catch (Exception e) { Trace.WriteLine("Error saving image (Banner): " + e); }
                    }
                    itemcount++;
                    title = item.Game.Title;
                    genre = item.Game.Genre;
                    path = columns[2];
                    link = item.Game.GamePage;
                    guid = columns[7];
                    double percent = itemcount / GameCount;
                    percentage = Convert.ToInt32(percent);
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    AddGameToOC()));
                    icon = null;
                    poster = null;
                    banner = null;
                }
            }
            else if (File.Exists($"./Resources/{email}GamesList.txt"))
            {
                string gameFile = $"./Resources/{email}GamesList.txt";
                string[] columns = new string[0];
                int itemcount = 0;
                int GameCount = File.ReadLines(gameFile).Count();
                foreach (var item in File.ReadAllLines(gameFile))
                {
                    columns = item.Split('|');
                    if (columns[4] != "")
                    {
                        try
                        {
                            string installDir = AppDomain.CurrentDomain.BaseDirectory;
                            string iconpath = installDir + "Resources/img/" + columns[4];
                            icon = new BitmapImage();
                            icon.BeginInit();
                            icon.UriSource = new Uri(iconpath);
                            icon.DecodePixelWidth = 80;
                            icon.CacheOption = BitmapCacheOption.OnLoad;
                            icon.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                            icon.EndInit();
                            icon.Freeze();
                        }
                        catch (Exception e) { Trace.WriteLine("Error saving image (Icon): " + e); }
                    }
                    if (columns[5] != "")
                    {
                        try
                        {
                            string installDir = AppDomain.CurrentDomain.BaseDirectory;
                            string posterpath = installDir + "Resources/img/" + columns[5];
                            poster = new BitmapImage();
                            poster.BeginInit();
                            poster.UriSource = new Uri(posterpath);
                            poster.DecodePixelWidth = 200;
                            poster.CacheOption = BitmapCacheOption.OnLoad;
                            poster.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                            poster.EndInit();
                            poster.Freeze();
                        }
                        catch (Exception e) { Trace.WriteLine("Error saving image (Poster): " + e); }
                    }
                    if (columns[6] != "")
                    {
                        try
                        {
                            string installDir = AppDomain.CurrentDomain.BaseDirectory;
                            string columnpath = installDir + "Resources/img/" + columns[6];
                            banner = new BitmapImage();
                            banner.BeginInit();
                            banner.UriSource = new Uri(columnpath);
                            banner.DecodePixelWidth = 300;
                            banner.CacheOption = BitmapCacheOption.OnLoad;
                            banner.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                            banner.EndInit();
                            banner.Freeze();
                        }
                        catch (Exception e) { Trace.WriteLine("Error saving image (Banner): " + e); }
                    }
                    itemcount++;
                    title = columns[0];
                    genre = columns[1];
                    path = columns[2];
                    link = columns[3];
                    guid = columns[7];
                    double percent = itemcount / GameCount;
                    percentage = Convert.ToInt32(percent);
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    AddGameToOC()));
                    icon = null;
                    poster = null;
                    banner = null;
                }
            }
        }
        public void AddGameToOC()
        {
            string threadState = Thread.CurrentThread.IsBackground.ToString();
            games.Add(new GameList
            {
                Title = title,
                Genre = genre,
                Path = path,
                Link = link,
                Icon = icon,
                Poster = poster,
                Banner = banner,
                Guid = guid
            });
        }
        public void AddStoreGameToOC()
        {
            string threadState = Thread.CurrentThread.IsBackground.ToString();
            storeGames.Add(new GameList
            {
                Title = storeTitle,
                Genre = storeGenre,
                Path = gamePage,
                Link = storeLink,
                Poster = storePoster,
                Guid = storeGuid
            });
        }
        public void AddGenresToOC()
        {
            genres.Add(new GenreList
            {
                Name = genreName,
                Guid = genreGuid
            });
        }
    }
}
