using System.Data.Entity;
using System.Data.SQLite;
using System.Reflection;
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

        public virtual DbSet<Entity.Version> Versions { get; set; }

        public virtual DbSet<Data> Results { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Entity.Version>().HasKey(t1 => t1.Id);

            Database.SetInitializer(new MainDbInitializer(modelBuilder));
        }

        public static SQLiteConnection GetConnectionString()
        {
            string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Assembly.GetEntryAssembly().GetName().Name, "Data");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Assembly.GetEntryAssembly().GetName().Name, "Data", "data.sqlite");
            var sqlConnection = new SQLiteConnection
            {
                ConnectionString = new SQLiteConnectionStringBuilder
                {
                    DataSource = fileName,
                    ForeignKeys = true
                }.ConnectionString
            };
            return sqlConnection;
        }
    }
}
