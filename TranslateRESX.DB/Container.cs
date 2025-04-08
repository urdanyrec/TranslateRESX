using System;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using TranslateRESX.Db.Repository;
using TranslateRESX.DB.Repository;

namespace TranslateRESX.DB
{
    public class Container : IContainer
    {
        private readonly MainDbContext _context;

        public IVersionRepository Versions { get; }

        public IDataRepository Results { get; set; }

        public ILanguageRepository Languages { get; set; }

        public string DatabaseDirectory { get; private set; }

        public Container()
        {
            SQLiteConnection sqLiteConnection = GetConnectionString();
            _context = new MainDbContext(sqLiteConnection);
            Versions = new VersionRepository(_context);
            Results = new DataRepository(_context);
            Languages = new LanguageRepository(_context);

            MainDbMigrations.Migration(this, _context);
        }
        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        private SQLiteConnection GetConnectionString()
        {
            DatabaseDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Assembly.GetEntryAssembly().GetName().Name, "Data");
            if (!Directory.Exists(DatabaseDirectory))
                Directory.CreateDirectory(DatabaseDirectory);

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