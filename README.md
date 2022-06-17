# GenerateCouponCapstone2022
## 1. About the Project
<br>
This project is intended to build a general Content Management System(CMS) for restaurant owners to generate Coupons for customers and send an email notifications to the customers. There are many functions included:

**Admin Role
- Manage Coupon (CRUD)
- Manage Restaurant (CRUD)
- Manage Customers (CRUD)
- Manage relationship between Coupon and Restaurant
- Manage relationship between Coupon and Customer
**Customer Role
- Customer create profile
- Customer choose restaurant and coupon from the list to buy

<br>
### 2. Authorization Permissions for Diffente Role

- Restaurant Entity – Create, Update, and Delete Functionality – Admin Role
- Coupon Entity – Create, Update, and Delete Functionality – Admin Role
- Email Entity – Create Functionality – Admin Role
- Customer Entity – All Functionality – Admin Role, Customer Role
- List all Restaurants, List all Coupons - Admin Role, Customer Role
<br>
<br>

## 3. How to Run This Project?

1. Clone the repository in Visual Studio
2. Open the project folder on your computer (e.g. File Explore for Windows Users)
3. Create an <App_Data> folder in the main project folder
4. Go back to Visual Studio and open Package Manager Console and run the query to build the database on your local server:
```
update-database
```
5. The project should set up


### 3.2 Before checking all the functions, manually add users data for Authorization!

1. open you database in the <SQL Server Object Explorer>, you will find all the tables for this project
2. Find <dbo.AspNetRoles> table and create two roles:
  - id:1, Name:"Admin"
  - id:2, Name:"Customer"
3. Run the project on your browser
4. Register with Email address and set Password
5. You Register with Customer and create customer profile and choose coupons from the drop-down
6. For Admin Login, Register with Email address and set Password. Then close the browser and go back to Visual Studio
7. Find <dbo.AspNetUsers> table and you will find the user you registered for Admin. Copy one of the user's id.
8. Find <dbo.AspNetUserRoles> table, and paste the id you copied into the first Userid row, then set the RoleId to "1"(1 = "Admin")
10. Now, you've set a user with "Admin" role. You are good to go!
