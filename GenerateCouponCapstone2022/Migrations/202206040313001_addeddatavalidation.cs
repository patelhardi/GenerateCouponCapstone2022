namespace GenerateCouponCapstone2022.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addeddatavalidation : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Coupons", "CouponCode", c => c.String(nullable: false));
            AlterColumn("dbo.Coupons", "Title", c => c.String(nullable: false));
            AlterColumn("dbo.Coupons", "Description", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Coupons", "Description", c => c.String());
            AlterColumn("dbo.Coupons", "Title", c => c.String());
            AlterColumn("dbo.Coupons", "CouponCode", c => c.String());
        }
    }
}
