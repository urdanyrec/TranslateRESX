using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslateRESX.DB
{
    public class MainDbContextHelper
    {
        public Dictionary<int, IList> Migrations { get; }

        public MainDbContextHelper()
        {
            Migrations = new Dictionary<int, IList>();
        }
    }
}
