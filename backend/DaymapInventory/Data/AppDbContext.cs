using DaymapInventory.Models;
using Microsoft.EntityFrameworkCore;

namespace DaymapInventory.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Tables
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemInstance> ItemInstances { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        // Junction tables
        public DbSet<ItemCategory> ItemCategories { get; set; }
        public DbSet<ItemTag> ItemTags { get; set; }
        public DbSet<CategoryTag> CategoryTags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ItemCategory composite primary key
            modelBuilder.Entity<ItemCategory>()
                .HasKey(ic => new { ic.ItemId, ic.CategoryId });

            modelBuilder.Entity<ItemCategory>()
                .HasOne(ic => ic.Item)
                .WithMany(i => i.ItemCategories)
                .HasForeignKey(ic => ic.ItemId);

            modelBuilder.Entity<ItemCategory>()
                .HasOne(ic => ic.Category)
                .WithMany(c => c.ItemCategories)
                .HasForeignKey(ic => ic.CategoryId);

            // ItemTag composite primary key
            modelBuilder.Entity<ItemTag>()
                .HasKey(it => new { it.ItemId, it.TagId });

            modelBuilder.Entity<ItemTag>()
                .HasOne(it => it.Item)
                .WithMany(i => i.ItemTags)
                .HasForeignKey(it => it.ItemId);

            modelBuilder.Entity<ItemTag>()
                .HasOne(it => it.Tag)
                .WithMany(t => t.ItemTags)
                .HasForeignKey(it => it.TagId);

            // CategoryTag composite primary key
            modelBuilder.Entity<CategoryTag>()
                .HasKey(ct => new { ct.CategoryId, ct.TagId });

            modelBuilder.Entity<CategoryTag>()
                .HasOne(ct => ct.Category)
                .WithMany(c => c.CategoryTags)
                .HasForeignKey(ct => ct.CategoryId);

            modelBuilder.Entity<CategoryTag>()
                .HasOne(ct => ct.Tag)
                .WithMany(t => t.CategoryTags)
                .HasForeignKey(ct => ct.TagId);

            // Transaction foreign keys
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Item)
                .WithMany(i => i.Transactions)
                .HasForeignKey(t => t.ItemId);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.ItemInstance)
                .WithMany()
                .HasForeignKey(t => t.ItemInstanceId)
                .IsRequired(false);

            // ItemInstance foreign key
            modelBuilder.Entity<ItemInstance>()
                .HasOne(ii => ii.Item)
                .WithMany(i => i.Instances)
                .HasForeignKey(ii => ii.ItemId);
        }
    }
}
