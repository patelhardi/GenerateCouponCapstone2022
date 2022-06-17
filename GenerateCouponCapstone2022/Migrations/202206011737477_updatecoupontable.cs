namespace GenerateCouponCapstone2022.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatecoupontable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Coupons", "CouponHasPic", c => c.Boolean(nullable: false));
            AddColumn("dbo.Coupons", "PicExtension", c => c.String());
            DropColumn("dbo.Coupons", "image");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Coupons", "image", c => c.Int(nullable: false));
            DropColumn("dbo.Coupons", "PicExtension");
            DropColumn("dbo.Coupons", "CouponHasPic");
        }
    }
}
