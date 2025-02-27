﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrismTest.Models.DataModels
{
    public class Game
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Genre { get; set; }
        public decimal Price { get; set; }
        public string GamePage { get; set; }
        public string BuyLink { get; set; }
        public string Poster { get; set; }
        public virtual ICollection<Purchase> Purchases { get; set; }
    }
}
