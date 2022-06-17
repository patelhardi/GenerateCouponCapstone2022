namespace GenerateCouponCapstone2022.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatecouponentity : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Coupons", "CouponHasPic");
            DropColumn("dbo.Coupons", "PicExtension");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Coupons", "PicExtension", c => c.String());
            AddColumn("dbo.Coupons", "CouponHasPic", c => c.Boolean(nullable: false));
        }
    }
}
