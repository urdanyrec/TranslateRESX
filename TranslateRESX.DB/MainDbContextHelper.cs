using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using TranslateRESX.Db.Entity;

namespace TranslateRESX.DB
{
    public class MainDbContextHelper
    {
        public Dictionary<int, IList> Migrations { get; }

        public MainDbContextHelper()
        {
            Migrations = new Dictionary<int, IList>();
            MigrationVersion2();
            MigrationVersion3();
        }

        private void MigrationVersion2()
        {
            IList steps = new List<string>();
            steps.Add("AlTER TABLE \"Data\" ADD COLUMN \"WaitTime\" INTEGER;");
            steps.Add("AlTER TABLE \"Data\" ADD COLUMN \"Service\" TEXT;"); 
            Migrations.Add(2, steps);
        }

        private void MigrationVersion3()
        {
            IList steps = new List<string>();
            steps.Add("CREATE TABLE IF NOT EXISTS \"Languages\" ([Id] INTEGER PRIMARY KEY, [LanguageName] TEXT NOT NULL, [LocalizationSuffix] TEXT NOT NULL, [LanguageCode] TEXT NOT NULL, [IsDefault] BIT DEFAULT 0)");
            Migrations.Add(3, steps);
        }

        public void AddDefaultLanguages(IContainer container)
        {
            List<Language> languages = new List<Language>
            {
                new Language { LanguageName = "Русский", LocalizationSuffix = "ru-RU", LanguageCode = "ru", IsDefault = true },
                new Language { LanguageName = "English", LocalizationSuffix = "en-US", LanguageCode = "en", IsDefault = true }
            };
            container.Languages.AddRange(languages);
            container.Complete();
        }
    }
}
