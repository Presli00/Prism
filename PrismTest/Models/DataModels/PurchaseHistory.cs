using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrismTest.Models.DataModels
{
    public class PurchaseHistory : Purchase
    {
        public decimal PurchaseAmount { get; set; }
    }
}
