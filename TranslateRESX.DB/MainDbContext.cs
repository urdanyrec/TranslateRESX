using System.Data.Entity;
using System.Data.SQLite;
using TranslateRESX.Db.Entity;
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

        public virtual DbSet<Language> Languages { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Version>().HasKey(t1 => t1.Id);
            modelBuilder.Entity<Data>().HasKey(t1 => t1.Id);
            modelBuilder.Entity<Language>().HasKey(t1 => t1.Id);

            Database.SetInitializer(new MainDbInitializer(modelBuilder));
        }
    }
}
