using TranslateRESX.DB.Entity;

namespace TranslateRESX.DB.Repository
{
    public interface IDataRepository : IRepository<Data>
    {
        IEnumerable<Data> GetByApiKey(string key);

        IEnumerable<Data> GetBySourceLanguage(string sourceLanguage);

        IEnumerable<Data> GetByDestinationLanguage(string destinationLanguage);
    }
}
