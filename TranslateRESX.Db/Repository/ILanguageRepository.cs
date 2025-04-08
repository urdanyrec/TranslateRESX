using System.Collections.Generic;
using TranslateRESX.Db.Entity;
using TranslateRESX.DB.Repository;

namespace TranslateRESX.Db.Repository
{
    public interface ILanguageRepository : IRepository<Language>
    {
        IEnumerable<Language> GetByLanguaheCode(string langCode);

        IEnumerable<Language> GetDefault();
    }
}
