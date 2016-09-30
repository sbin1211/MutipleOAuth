namespace MutipleOAuth.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class third : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OAuthApp", "ClientKey", c => c.String(nullable: false));
            DropColumn("dbo.OAuthApp", "AppId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OAuthApp", "AppId", c => c.String(nullable: false));
            DropColumn("dbo.OAuthApp", "ClientKey");
        }
    }
}
