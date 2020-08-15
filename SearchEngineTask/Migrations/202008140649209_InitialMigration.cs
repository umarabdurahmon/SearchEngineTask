namespace SearchEngineTask.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Searches",
                c => new
                    {
                        SearchId = c.Int(nullable: false, identity: true),
                        SearchText = c.String(),
                        SearchEngineName = c.String(),
                    })
                .PrimaryKey(t => t.SearchId);
            
            CreateTable(
                "dbo.SearchResults",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        Url = c.String(nullable: false),
                        Description = c.String(defaultValue: ""),
                        SearchId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Searches", t => t.SearchId, cascadeDelete: true)
                .Index(t => t.SearchId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SearchResults", "SearchId", "dbo.Searches");
            DropIndex("dbo.SearchResults", new[] { "SearchId" });
            DropTable("dbo.SearchResults");
            DropTable("dbo.Searches");
        }
    }
}
