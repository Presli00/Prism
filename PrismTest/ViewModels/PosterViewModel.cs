﻿using PrismTest.Models;
using System.Collections.ObjectModel;

namespace PrismTest.ViewModels
{
    internal class PosterViewModel
    {
        public static ObservableCollection<GameList> PosterViewOC { get; set; }
        public static ObservableCollection<GenreList> GenreListOC { get; set; }
        private LoadSearch ls = new LoadSearch();
        public static ObservableCollection<SearchResults> SearchList { get; set; }

        public void LoadSearch(string gametitle, string imagetype, string searchstring, int offset)
        {
            ls.Search(gametitle, imagetype, searchstring, offset);
            SearchList = ls.SearchList;
        }
        public void LoadGames()
        {
            PosterViewOC = MainWindow.GameListMW;
            GenreListOC = MainWindow.GenreListMW;
        }
    }
}