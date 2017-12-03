namespace AgileGardeningApp.ServiceRepository.SqlDbImpl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitDb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Plants",
                c => new
                    {
                        PlantId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Types = c.String(nullable: false),
                        Keywords = c.String(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.PlantId);
            
            CreateTable(
                "dbo.UsersPlantsInfoes",
                c => new
                    {
                        UsersPlantsInfoId = c.Int(nullable: false, identity: true),
                        IsFavourite = c.Boolean(nullable: false),
                        Plant_PlantId = c.Int(),
                        User_UserId = c.Int(),
                    })
                .PrimaryKey(t => t.UsersPlantsInfoId)
                .ForeignKey("dbo.Plants", t => t.Plant_PlantId)
                .ForeignKey("dbo.Users", t => t.User_UserId)
                .Index(t => t.Plant_PlantId)
                .Index(t => t.User_UserId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        EmailAddress = c.String(nullable: false),
                        Password = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UsersPlantsInfoes", "User_UserId", "dbo.Users");
            DropForeignKey("dbo.UsersPlantsInfoes", "Plant_PlantId", "dbo.Plants");
            DropIndex("dbo.UsersPlantsInfoes", new[] { "User_UserId" });
            DropIndex("dbo.UsersPlantsInfoes", new[] { "Plant_PlantId" });
            DropTable("dbo.Users");
            DropTable("dbo.UsersPlantsInfoes");
            DropTable("dbo.Plants");
        }
    }
}
