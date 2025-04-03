using System.Data.Entity;
using System.Data.SQLite;
using TranslateRESX.DB.Entity;

namespace TranslateRESX.DB
{
    public class MainDbContext : DbContext
    {
        public MainDbContext(SQLiteConnection connectionString) : base(connectionString, true)
        {
            OnModelCreating(new DbModelBuilder());
            Database.Initialize(true);
        }

        public virtual DbSet<Version> Versions { get; set; }

        public virtual DbSet<Data> Results { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Version>().HasKey(t1 => t1.Id);
            modelBuilder.Entity<Data>().HasKey(t1 => t1.Id);

            Database.SetInitializer(new MainDbInitializer(modelBuilder));
        }
    }
}
