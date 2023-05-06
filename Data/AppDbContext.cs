using All_type_input_database.Models;
using Microsoft.EntityFrameworkCore;

namespace All_type_input_database.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Movie_data_model> Movie_Table { get; set; }
    }
}
