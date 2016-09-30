namespace MutipleOAuth.Migrations
{
    using Data;
    using Externsions;
    using FizzWare.NBuilder;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Migrations.Model;
    using System.Data.Entity.SqlServer;
    using System.Linq;
    using System.Threading;

    internal sealed class Configuration : DbMigrationsConfiguration<AppDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            SetSqlGenerator("System.Data.SqlClient", new CustomSqlServerMigrationSqlGenerator());
        }

        protected override void Seed(AppDbContext context)
        {
            var apps = Builder<OAuthApp>.CreateListOfSize(100).All()
                    .With(c => c.ClientKey = Guid.NewGuid().ToString())
                    .With(c => c.Tenant = Guid.NewGuid().ToString())
                    .With(c => c.ClientSecrect = Guid.NewGuid().ToString())
                    .With(c => c.Provider = RandomPorvider())
                    .Build();

            var predefineApp = new OAuthApp()
            {
                ClientKey = "261641239844-qhs4ljqv8mo8mbc0ni7fmbb23se9ksnu.apps.googleusercontent.com",
                ClientSecrect = "u6Y0eNGu-v3gK7L-IiaNsMoS",
                Provider = "Google",
                Tenant = "webstore",

            };
            apps.Add(predefineApp);

            predefineApp = new OAuthApp()
            {
                ClientKey = "740516412651-pni9sk3hmcu3ongf94mom952ple4tumt.apps.googleusercontent.com",
                ClientSecrect = "8cMwmwF0YXt7EGUKog9vIj3G",
                Provider = "Google",
                Tenant = "Webclient",

            };
            apps.Add(predefineApp);


            context.OAuthApps.AddIfNoExist(c => c.Id, apps.ToArray());

        }
        private string RandomPorvider()
        {
            Thread.Sleep(10);
            var rnd = new Random();
            var seed = rnd.Next(0, 3);
            switch (seed)
            {
                case 0:
                    return "Google";
                case 1:
                    return "Facebook";
                case 2:
                    return "Microsoft";
                default:
                    return "Microsoft";
            }
        }
        internal class CustomSqlServerMigrationSqlGenerator : SqlServerMigrationSqlGenerator
        {
            protected override void Generate(AddColumnOperation addColumnOperation)
            {
                SeCustomColumn(addColumnOperation.Column);

                base.Generate(addColumnOperation);
            }

            protected override void Generate(CreateTableOperation createTableOperation)
            {
                SetCustomColumns(createTableOperation.Columns);

                base.Generate(createTableOperation);
            }

            private static void SetCustomColumns(IEnumerable<ColumnModel> columns)
            {
                foreach (var columnModel in columns)
                {
                    SeCustomColumn(columnModel);
                }
            }

            private static void SeCustomColumn(PropertyModel column)
            {
                if (column.Name == "CreatedOn")
                {
                    column.DefaultValueSql = "GETUTCDATE()";

                }
            }
        }
    }
}
