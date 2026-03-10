using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
namespace HaritaUygulamasi.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("name=PostgreConnection") { }
        public DbSet<HaritaNesnesi> HaritaNesneleri { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}