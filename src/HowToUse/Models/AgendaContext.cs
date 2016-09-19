using System.Threading;
using System.Threading.Tasks;
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

        public override int SaveChanges() {
            // ensure auto history
            this.EnsureAutoHistory();

            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess) {
            // ensure auto history
            this.EnsureAutoHistory();

            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken)) {
            // ensure auto history
            this.EnsureAutoHistory();

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken)) {
            // ensure auto history
            this.EnsureAutoHistory();

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
