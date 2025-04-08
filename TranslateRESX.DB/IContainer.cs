using System;
using TranslateRESX.Db.Repository;
using TranslateRESX.DB.Repository;

namespace TranslateRESX.DB
{
    public interface IContainer : IDisposable
    {
        IVersionRepository Versions { get; }

        IDataRepository Results { get; }

        ILanguageRepository Languages { get; }

        string DatabaseDirectory { get; }

        int Complete();
    }
}
