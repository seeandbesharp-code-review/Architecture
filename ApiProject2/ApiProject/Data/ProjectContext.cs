using ApiProject.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiProject.Data
{
    public class ProjectContext:DbContext
    {
        public ProjectContext(DbContextOptions<ProjectContext> options) : base(options)
        {
        }
        //יצירת הטבלאות
        public DbSet<UserModel> users { get; set; }
        public DbSet<GiftModel> gifts { get; set; }
        public DbSet<DonorModel> donors { get; set; }
        public DbSet<CartModel> carts { get; set; }
        public DbSet<CartItemModel> cartItems { get; set; }
        public DbSet<PurchaseModel> purchaseItems { get; set; }
        public DbSet<RaffleResultModel> raffleResults { get; set; }
        public DbSet<CategoryModel> categories { get; set; }


    }
}


