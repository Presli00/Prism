using PrismTest.Models;
using System.Collections.ObjectModel;

namespace PrismTest.ViewModels
{
    internal class SettingsViewModel
    {
        public static ObservableCollection<GenreList> GenreListOC { get; set; }

        public void LoadGenres()
        {
            GenreListOC = MainWindow.GenreListMW;
        }
    }
}