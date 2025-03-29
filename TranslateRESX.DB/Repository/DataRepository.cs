using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TranslateRESX.DB.Entity;

namespace TranslateRESX.DB.Repository
{
    public class DataRepository : Repository<Data>, IDataRepository
    {
        public DataRepository(DbContext context) : base(context)
        {
        }

        public MainDbContext MainContext => Context as MainDbContext;

        public IEnumerable<Data> GetByApiKey(string key)
        {
            return MainContext.Results.Where(m => m.ApiKey == key).OrderByDescending(m => m.Id);
        }

        public IEnumerable<Data> GetBySourceLanguage(string sourceLanguage)
        {
            return MainContext.Results.Where(m => m.SourceLanguage == sourceLanguage).OrderByDescending(m => m.Id);
        }

        public IEnumerable<Data> GetByDestinationLanguage(string destinationLanguage)
        {
            return MainContext.Results.Where(m => m.DestinationLanguage == destinationLanguage).OrderByDescending(m => m.Id);
        }
    }
}
