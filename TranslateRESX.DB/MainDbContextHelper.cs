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
            MigrationVersion2();
        }

        private void MigrationVersion2()
        {
            IList steps = new List<string>();
            steps.Add("AlTER TABLE \"Data\" ADD COLUMN \"WaitTime\" INTEGER;");
            steps.Add("AlTER TABLE \"Data\" ADD COLUMN \"Service\" TEXT;"); 
            Migrations.Add(2, steps);
        }
    }
}
