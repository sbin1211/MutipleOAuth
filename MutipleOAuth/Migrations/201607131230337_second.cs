namespace MutipleOAuth.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class second : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OAuthApp", "Tenant", c => c.String(nullable: false));
            DropColumn("dbo.OAuthApp", "StoreKey");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OAuthApp", "StoreKey", c => c.String(nullable: false));
            DropColumn("dbo.OAuthApp", "Tenant");
        }
    }
}
