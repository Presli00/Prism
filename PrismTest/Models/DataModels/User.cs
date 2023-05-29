using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrismTest.Models.DataModels
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        public string Salt { get; set; }
        public decimal WalletBalance { get; set; }
        public string Region { get; set; }

        public ICollection<Purchase> Purchases { get; set; }
    }
}
