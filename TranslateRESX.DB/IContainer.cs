using TranslateRESX.DB.Repository;

namespace TranslateRESX.DB
{
    public interface IContainer : IDisposable
    {
        IVersionRepository Versions { get; }

        IDataRepository Results { get; }

        int Complete();
    }
}
