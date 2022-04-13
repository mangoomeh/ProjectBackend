using Microsoft.EntityFrameworkCore;
using ProjectBackend.Models;

namespace ProjectBackend.Data.Context
{
    public class BloggerContext : DbContext
    {
        public BloggerContext(DbContextOptions<BloggerContext> options) 
            : base (options)
        {
        }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.Comments)
                .WithOne(c => c.User)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
