namespace AgileGardeningApp.ServiceRepository.SqlDbImpl.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedLogintokenToUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "LoginToken", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "LoginToken");
        }
    }
}
