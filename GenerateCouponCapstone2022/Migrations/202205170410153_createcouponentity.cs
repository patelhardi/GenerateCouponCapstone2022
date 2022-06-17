namespace GenerateCouponCapstone2022.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class createcouponentity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Coupons",
                c => new
                    {
                        CouponID = c.Int(nullable: false, identity: true),
                        CouponCode = c.String(),
                        Title = c.String(),
                        ExpiryDate = c.DateTime(nullable: false),
                        image = c.Int(nullable: false),
                        Description = c.String(),
                        RestaurantID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CouponID)
                .ForeignKey("dbo.Restaurants", t => t.RestaurantID, cascadeDelete: true)
                .Index(t => t.RestaurantID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Coupons", "RestaurantID", "dbo.Restaurants");
            DropIndex("dbo.Coupons", new[] { "RestaurantID" });
            DropTable("dbo.Coupons");
        }
    }
}
