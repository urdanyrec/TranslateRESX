using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using TranslateRESX.DB.Repository;
using TranslateRESX.DB;
using TranslateRESX.Db.Entity;

namespace TranslateRESX.Db.Repository
{
    public class LanguageRepository : Repository<Language>, ILanguageRepository
    {
        public LanguageRepository(DbContext context) : base(context)
        {
        }

        public MainDbContext MainContext => Context as MainDbContext;

        public IEnumerable<Language> GetByLanguaheCode(string langCode)
        {
            return MainContext.Languages.Where(m => m.LanguageCode == langCode).OrderByDescending(m => m.Id);
        }

        public IEnumerable<Language> GetDefault()
        {
            return MainContext.Languages.Where(m => m.IsDefault).OrderByDescending(m => m.Id);
        }
    }
}
