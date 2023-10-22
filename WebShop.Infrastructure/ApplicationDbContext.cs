using Microsoft.EntityFrameworkCore;
using WebShop.Domain.Entities;

namespace WebShop.Infrastructure
{
    public sealed class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<User> Users { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<User>().HasData(SeedData.UserSeed.Data);

        //    // CreatedOn
        //    modelBuilder.Entity<User>().Property(x => x.CreatedOn).HasDefaultValueSql("GETUTCDATE()");


        //    // Unique indexes
        //    modelBuilder.Entity<User>()
        //      .HasMany(e => e.RefreshTokens)
        //      .WithOne(d => d.User)
        //      .OnDelete(DeleteBehavior.Restrict);

        //    base.OnModelCreating(modelBuilder);
        //}

        //public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        //{
        //    ChangeTracker.DetectChanges();
        //    OnBeforeSaving();

        //    return await base.SaveChangesAsync(cancellationToken);
        //}

        //public override int SaveChanges()
        //{
        //    ChangeTracker.DetectChanges();
        //    OnBeforeSaving();

        //    return base.SaveChanges();
        //}

        //private void OnBeforeSaving()
        //{
        //    var updatedEntries = ChangeTracker.Entries().Where(x => x.State != EntityState.Unchanged && x.State != EntityState.Detached && x.State != EntityState.Added);

        //    foreach (var entry in updatedEntries)
        //    {
        //        if (entry.State == EntityState.Deleted)
        //        {
        //            if (entry.Entity is ISoftDelete deleteDate)
        //            {
        //                deleteDate.DeletedOn = DateTime.UtcNow;
        //                entry.State = EntityState.Modified;
        //            }
        //        }

        //        if (entry.Entity is IBaseEntity updatedDate)
        //        {
        //            updatedDate.UpdatedOn = DateTime.UtcNow;
        //            //entry.State = EntityState.Modified;
        //        }

        //        if (entry.Entity is ISoftDelete updatedDate2)
        //        {
        //            updatedDate2.UpdatedOn = DateTime.UtcNow;
        //        }
        //    }
        //}
    }
}
