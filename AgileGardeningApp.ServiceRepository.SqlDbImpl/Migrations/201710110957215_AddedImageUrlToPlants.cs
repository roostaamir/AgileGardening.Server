namespace AgileGardeningApp.ServiceRepository.SqlDbImpl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedImageUrlToPlants : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Plants", "ImageUrl", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Plants", "ImageUrl");
        }
    }
}
