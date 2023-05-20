using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrismTest.Models.DataModels
{
    public class ModelContext : DbContext
    {
        public ModelContext(DbContextOptions<ModelContext> options) :
            base(options)
        {

        }

        public ModelContext()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("server=localhost\\sqlexpress;database=PrismDatabase;trusted_connection=true");
            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Purchase> Purchase { get; set; }
        public DbSet<PurchaseHistory> PurchaseHistory { get; set; }
    }
}
