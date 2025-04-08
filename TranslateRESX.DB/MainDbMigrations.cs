using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;

namespace TranslateRESX.DB
{
    public static class MainDbMigrations
    {
        private static readonly int _requiredDatabaseVersion = 3;

        public static void Migration(Container container, MainDbContext dbContext)
        {
            //Если таблицы версий не существует, значит это пустая БД
            var isExist = CheckTableVersionExist(dbContext);
            if (!isExist)
                return;

            //Если есть таблица версий - смотрим максимальную цифру
            var currentVersion = isExist ? container.Versions.GetMaxVersion() : 0;
            if (currentVersion >= _requiredDatabaseVersion)
                return;

            MainDbContextHelper dbContextHelper = new MainDbContextHelper();
            while (currentVersion < _requiredDatabaseVersion)
            {
                currentVersion++;
                if (!dbContextHelper.Migrations.ContainsKey(currentVersion))
                    continue;

                foreach (string migration in dbContextHelper.Migrations[currentVersion])
                {
                    try
                    {
                        dbContext.Database.ExecuteSqlCommand(migration);

                        if (currentVersion == 3)
                            dbContextHelper.AddDefaultLanguages(container);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }

                dbContext.Versions.Add(new Entity.Version { Id = currentVersion, Number = currentVersion });
                dbContext.SaveChanges();
            }
        }

        private static bool CheckTableVersionExist(DbContext context)
        {
            var query = context.Database.SqlQuery<int?>(@"SELECT COUNT(*) FROM sqlite_master AS T WHERE T.Name = 'Versions'");
            bool exists = query?.SingleOrDefault() > 0;
            return exists;
        }
    }
}
