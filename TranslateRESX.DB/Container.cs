using System.Data.SQLite;
using TranslateRESX.DB.Repository;

namespace TranslateRESX.DB
{
    public class Container : IContainer
    {
        private readonly MainDbContext _context;

        public IVersionRepository Versions { get; }

        public IDataRepository Results { get; set; }

        public Container()
        {
            SQLiteConnection sqLiteConnection = MainDbContext.GetConnectionString();
            _context = new MainDbContext(sqLiteConnection);
            Versions = new VersionRepository(_context);
            Results = new DataRepository(_context);

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
    }
}