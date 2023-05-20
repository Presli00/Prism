using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrismTest.Models.DataModels
{
    public class Purchase
    {
        public int PurchaseId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int GameId { get; set; }
        public Game Game { get; set; }
        public DateTime PurchaseDate { get; set; }
    }
}
