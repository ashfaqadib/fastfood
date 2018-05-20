# fastfood
A food ordering website called “Fast-Food”, developed in ASP.NET MVC framework.
You might want to edit the connection string in "WebConfig" file placed inside Project folder. Change the "MyDb" portion of the "Data Source=(LocalDB)\MyDb;" according to your sql server instance.
If somehow you are not getting any default data in the database, uncomment the following line-

MyDbContext dbContext = MyDbContext.GetDbContext(); 
dbContext.AddToDb();

in the "Application_Start()" funtion inside "Global.asax.cs" file. Run the application once after uncommenting the line and then comment out the line again, otherwise the project will stop working.
