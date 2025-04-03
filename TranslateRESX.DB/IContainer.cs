using System;
using TranslateRESX.DB.Repository;

namespace TranslateRESX.DB
{
    public interface IContainer : IDisposable
    {
        IVersionRepository Versions { get; }

        IDataRepository Results { get; }

        string DatabaseDirectory { get; }

        int Complete();
    }
}
