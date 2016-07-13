using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace MutipleOAuth.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() : base("defaultDB") { }
        public IDbSet<OAuthApp> OAuthApps { get; set; }

        public override Task<int> SaveChangesAsync()
        {
            try
            {
                SetUpdatedOn();
                return base.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                throw ex;
            }
        }
        public override int SaveChanges()
        {

            SetUpdatedOn();

            return base.SaveChanges();
        }

        private void SetUpdatedOn()
        {
            var entities = ChangeTracker.Entries().Where(x => x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));


            foreach (var entity in entities)
            {
                ((BaseEntity)entity.Entity).UpdatedOn = DateTime.UtcNow;

            }
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Types<BaseEntity>().Configure(c =>
            {
                c.Property(x => x.CreatedOn)
                .HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Computed);
            });

            var oauthAppTable = modelBuilder.Entity<OAuthApp>().ToTable("OAuthApp");
            oauthAppTable.Property(x => x.Provider).IsRequired();
            oauthAppTable.Property(x => x.StoreKey).IsRequired();
            oauthAppTable.Property(x => x.AppId).IsRequired();
            oauthAppTable.Property(x => x.ClientSecrect).IsRequired();

            base.OnModelCreating(modelBuilder);
        }
    }
}