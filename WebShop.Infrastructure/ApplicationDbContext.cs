using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShop.Domain.Entities;
using System.Reflection.Emit;
using System.Xml.Linq;

namespace WebShop.Infrastructure
{
    public sealed class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Image> Images { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(SeedData.UserSeed.Data);
            modelBuilder.Entity<Category>().HasData(SeedData.CategorySeed.Data);
            modelBuilder.Entity<News>().HasData(SeedData.NewsSeed.Data);
            modelBuilder.Entity<Faq>().HasData(SeedData.FaqSeed.Data);
            modelBuilder.Entity<Comments>().HasData(SeedData.CommentSeed.Data);
            modelBuilder.Entity<File>().HasData(SeedData.FileSeed.Data);

            // CreatedOn
            modelBuilder.Entity<User>().Property(x => x.CreatedOn).HasDefaultValueSql("GETUTCDATE()");
            modelBuilder.Entity<RefreshToken>().Property(x => x.CreatedOn).HasDefaultValueSql("GETUTCDATE()");
            modelBuilder.Entity<Log>().Property(x => x.CreatedOn).HasDefaultValueSql("GETUTCDATE()");
            modelBuilder.Entity<UserFcmToken>().Property(x => x.CreatedOn).HasDefaultValueSql("GETUTCDATE()");
            modelBuilder.Entity<Faq>().Property(x => x.CreatedOn).HasDefaultValueSql("GETUTCDATE()");
            modelBuilder.Entity<Category>().Property(x => x.CreatedOn).HasDefaultValueSql("GETUTCDATE()");
            modelBuilder.Entity<News>().Property(x => x.CreatedOn).HasDefaultValueSql("GETUTCDATE()");
            modelBuilder.Entity<Comments>().Property(x => x.CreatedOn).HasDefaultValueSql("GETUTCDATE()");
            modelBuilder.Entity<Question>().Property(x => x.CreatedOn).HasDefaultValueSql("GETUTCDATE()");
            modelBuilder.Entity<UserCategory>().Property(x => x.CreatedOn).HasDefaultValueSql("GETUTCDATE()");
            modelBuilder.Entity<File>().Property(x => x.CreatedOn).HasDefaultValueSql("GETUTCDATE()");
            modelBuilder.Entity<Notification>().Property(x => x.CreatedOn).HasDefaultValueSql("GETUTCDATE()");


            // Unique indexes
            modelBuilder.Entity<User>()
              .HasMany(e => e.RefreshTokens)
              .WithOne(d => d.User)
              .OnDelete(DeleteBehavior.Restrict);

            // Filter deleted
            modelBuilder.Entity<User>().HasQueryFilter(x => x.DeletedOn == null);

            base.OnModelCreating(modelBuilder);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ChangeTracker.DetectChanges();
            OnBeforeSaving();

            return await base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            ChangeTracker.DetectChanges();
            OnBeforeSaving();

            return base.SaveChanges();
        }

        private void OnBeforeSaving()
        {
            var updatedEntries = ChangeTracker.Entries().Where(x => x.State != EntityState.Unchanged && x.State != EntityState.Detached && x.State != EntityState.Added);

            foreach (var entry in updatedEntries)
            {
                if (entry.State == EntityState.Deleted)
                {
                    if (entry.Entity is ISoftDelete deleteDate)
                    {
                        deleteDate.DeletedOn = DateTime.UtcNow;
                        entry.State = EntityState.Modified;
                    }
                }

                if (entry.Entity is IBaseEntity updatedDate)
                {
                    updatedDate.UpdatedOn = DateTime.UtcNow;
                    //entry.State = EntityState.Modified;
                }

                if (entry.Entity is ISoftDelete updatedDate2)
                {
                    updatedDate2.UpdatedOn = DateTime.UtcNow;
                }
            }
        }
    }
}
