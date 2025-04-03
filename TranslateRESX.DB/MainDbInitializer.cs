using SQLite.CodeFirst;
using System.Data.Entity;
using System.Linq;

namespace TranslateRESX.DB
{
    public class MainDbInitializer : SqliteCreateDatabaseIfNotExists<MainDbContext>
    {
        public MainDbInitializer(DbModelBuilder modelBuilder) : base(modelBuilder)
        {
        }

        protected override void Seed(MainDbContext context)
        {
            if (!context.Versions.Any())
            {
                context.Set<Entity.Version>().Add(new Entity.Version { Id = 1, Number = 1 });
                context.Set<Entity.Version>().Add(new Entity.Version { Id = 2, Number = 2 });
                context.SaveChanges();
            }
        }
    }
}
