using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslateRESX.DB.Entity
{
    [Serializable]
    public class Version : IEntity
    {
        public int Id { get; set; }

        public int Number { get; set; }
    }
}
