using System.Data.Entity;
using System.Linq;

namespace TranslateRESX.DB.Repository
{
    public class VersionRepository : Repository<Entity.Version>, IVersionRepository
    {
        public VersionRepository(DbContext context) : base(context)
        {
        }

        public MainDbContext MainContext => Context as MainDbContext;

        public int GetMaxVersion()
        {
            return MainContext.Versions.Any() ? MainContext.Versions.Max(v => v.Number) : 0;
        }
    }
}
