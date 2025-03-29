using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslateRESX.DB.Repository
{
    public interface IVersionRepository : IRepository<Entity.Version>
    {
        int GetMaxVersion();
    }
}
