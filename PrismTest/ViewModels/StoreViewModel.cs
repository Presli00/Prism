using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrismTest.Models;

namespace PrismTest.ViewModels
{
    internal class StoreViewModel
    {
        public static ObservableCollection<GameList> PosterViewOC { get; set; }
        private LoadSearch ls = new LoadSearch();
        public static ObservableCollection<SearchResults> SearchList { get; set; }

        public void LoadSearch(string gameTitle, string imageType, string searchString, int offset)
        {
            ls.Search(gameTitle, imageType, searchString, offset);
            SearchList = ls.SearchList;
        }
        public void LoadGames()
        {
            PosterViewOC = MainWindow.GameListMW;
        }
    }
}
