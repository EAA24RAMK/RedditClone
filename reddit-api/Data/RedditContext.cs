using Microsoft.EntityFrameworkCore;
using Model;

namespace Data
{
    public class RedditContext : DbContext
    {
        public DbSet<Post> Posts => Set<Post>();
        public DbSet<Comment> Comments => Set<Comment>();


        public RedditContext (DbContextOptions<RedditContext> options)
            : base(options)
        {
            // Den her er tom. Men ": base(options)" sikre at constructor
            // på DbContext super-klassen bliver kaldt.
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Sikrer at EF forstår 1 post har mange comments
            modelBuilder.Entity<Post>()
                .HasMany(p => p.Comments)
                .WithOne(c => c.Post)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade); // sletter kommentarer, hvis posten slettes
            
            // Sørger for at title er påkrævet
            modelBuilder.Entity<Post>()
                .Property(p => p.Title)
                .IsRequired();
            
            // Sørger for at Text ikke er påkrævet, da vi også kan have URL
            modelBuilder.Entity<Post>()
                .Property(p => p.Text)
                .IsRequired(false);
            
            modelBuilder.Entity<Post>()
                .Property(p => p.Url)
                .IsRequired(false);
        }
    }
}