using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebApplication1.Models;

namespace WebApplication1.Data

{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<AppUsers> Users { get; set; }

    }
}
