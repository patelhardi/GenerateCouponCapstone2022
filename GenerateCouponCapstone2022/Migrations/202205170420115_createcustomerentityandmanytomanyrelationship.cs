namespace GenerateCouponCapstone2022.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class createcustomerentityandmanytomanyrelationship : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        CustomerID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Email = c.String(),
                        Phone = c.String(),
                        IsSubscribed = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.CustomerID);
            
            CreateTable(
                "dbo.CustomerCoupons",
                c => new
                    {
                        Customer_CustomerID = c.Int(nullable: false),
                        Coupon_CouponID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Customer_CustomerID, t.Coupon_CouponID })
                .ForeignKey("dbo.Customers", t => t.Customer_CustomerID, cascadeDelete: true)
                .ForeignKey("dbo.Coupons", t => t.Coupon_CouponID, cascadeDelete: true)
                .Index(t => t.Customer_CustomerID)
                .Index(t => t.Coupon_CouponID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CustomerCoupons", "Coupon_CouponID", "dbo.Coupons");
            DropForeignKey("dbo.CustomerCoupons", "Customer_CustomerID", "dbo.Customers");
            DropIndex("dbo.CustomerCoupons", new[] { "Coupon_CouponID" });
            DropIndex("dbo.CustomerCoupons", new[] { "Customer_CustomerID" });
            DropTable("dbo.CustomerCoupons");
            DropTable("dbo.Customers");
        }
    }
}
