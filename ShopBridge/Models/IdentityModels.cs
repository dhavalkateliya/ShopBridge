using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using ShopBridge.Models;

namespace ShopBridge.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
            : base("name=DefaultConnection")
        {
        }

        public DbSet<Product> Product { get; set; }


        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>().ToTable("Product");
        }
    }
}