using Microsoft.EntityFrameworkCore;

namespace HowToUse.Models {
    internal static class AgendaConfiguration {
        public static ModelBuilder ConfigureAgenda(this ModelBuilder modelBuilder) {
            modelBuilder.Entity<Agenda>(b => {
                b.Ignore(e => e.Items);
                b.Property(e => e.Schedule).HasMaxLength(1024);
            });

            return modelBuilder;
        }
    }

    public class AgendaContext : DbContext {
        public AgendaContext(DbContextOptions<AgendaContext> options) : base(options) { }

        public DbSet<Agenda> Agenda { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            modelBuilder.EnableAutoHistory();
            modelBuilder.ConfigureAgenda();
        }
    }
}
