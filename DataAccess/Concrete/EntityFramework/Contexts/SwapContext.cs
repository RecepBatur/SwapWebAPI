using Core.Entities.Concrete;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete.EntityFramework.Contexts
{
    public class SwapContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString: @"Server=RECEP;Database=SpotifyDbRecep;User ID=sa;Password=;TrustServerCertificate=True");
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserOperationClaim> UserOperationClaims { get; set; }
        public DbSet<OperationClaim> OperationClaims { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<BuyOrder> BuyOrders { get; set; }
        public DbSet<SalesOrder> SalesOrders { get; set; }
        public DbSet<Parity> Parities { get; set; }
        public DbSet<Coin> Coins { get; set; }
        public DbSet<CompanyWallet> CompanyWallets { get; set; }
        public DbSet<OrderStatus> OrderStatues { get; set; }
    }
}
