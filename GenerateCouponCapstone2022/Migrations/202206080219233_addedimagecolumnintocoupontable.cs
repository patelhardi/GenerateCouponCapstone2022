namespace GenerateCouponCapstone2022.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedimagecolumnintocoupontable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Coupons", "Image", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Coupons", "Image");
        }
    }
}
