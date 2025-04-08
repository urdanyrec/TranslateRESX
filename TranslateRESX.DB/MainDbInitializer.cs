using SQLite.CodeFirst;
using System.Data.Entity;
using System.Linq;
using TranslateRESX.Db.Entity;

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
                context.Set<Entity.Version>().Add(new Entity.Version { Id = 3, Number = 3 });
                context.SaveChanges();
            }

            if (context.Languages == null || !context.Languages.Any())
            {
                context.Set<Language>().Add(new Language { LanguageName = "Русский", LocalizationSuffix = "ru-RU", LanguageCode = "ru", IsDefault = true });
                context.Set<Language>().Add(new Language { LanguageName = "English", LocalizationSuffix = "en-US", LanguageCode = "en", IsDefault = true });
                context.SaveChanges();
            }
        }
    }
}
