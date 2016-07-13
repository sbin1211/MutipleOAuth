namespace MutipleOAuth.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class first : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OAuthApp",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AppId = c.String(nullable: false),
                        ClientSecrect = c.String(nullable: false),
                        StoreKey = c.String(nullable: false),
                        Provider = c.String(nullable: false),
                        UpdatedOn = c.DateTime(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.OAuthApp");
        }
    }
}
