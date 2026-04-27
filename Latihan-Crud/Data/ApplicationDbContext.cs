using Latihan_Crud.Models;
using Microsoft.EntityFrameworkCore;

namespace Latihan_Crud.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

        public DbSet<Product> Products { get; set; }
    }
}
