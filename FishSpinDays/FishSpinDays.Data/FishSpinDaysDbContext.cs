namespace FishSpinDays.Data
{
    using FishSpinDays.Models;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    public class FishSpinDaysDbContext : IdentityDbContext<User>
    {
        public FishSpinDaysDbContext(DbContextOptions<FishSpinDaysDbContext> options)
            : base(options)
        {
        }
        public DbSet<MainSection> MainSections { get; set; }

        public DbSet<Section> Sections { get; set; }

        public DbSet<Publication> Publications { get; set; }

        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder
                .Entity<User>()
                .HasMany(u => u.Publications)
                .WithOne(p => p.Author)
                .HasForeignKey(p => p.AuthorId);

            builder
                .Entity<User>()
                .HasMany(u => u.Comments)
                .WithOne(p => p.Author)
                .HasForeignKey(p => p.AuthorId);

            builder
                .Entity<Section>()
                .HasMany(t => t.Publications)
                .WithOne(p => p.Section)
                .HasForeignKey(p => p.SectionId);

            builder
               .Entity<MainSection>()
               .HasMany(t => t.Sections)
               .WithOne(p => p.MainSection)
               .HasForeignKey(p => p.MainSectionId);
                       
            base.OnModelCreating(builder);
        }
    }
}
