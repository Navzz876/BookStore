using BookStore.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Data
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) 
        {

        }
        public DbSet<Category> Categories { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id=1, Name="Drama", DisplayOrder=1},
                 new Category { Id = 2, Name = "Horror", DisplayOrder = 2 },
                  new Category { Id = 3, Name = "Sci-fi", DisplayOrder = 3 },
                   new Category { Id = 4, Name = "Fiction", DisplayOrder = 4 },
                    new Category { Id = 5, Name = "Comics", DisplayOrder = 5 }

                );
        }
    }
}
