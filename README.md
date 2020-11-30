<h1>Food Ordering System</h1>
</br>
<p>A system that you can order food from any restaurant</p>
<h4>Used :</h4>
<p>-ASP.NET MVC
  
-MSSQL

-JQuery/AJAX

-HTML/CSS

Roles: Admin, Restaurant, User</p>

<h4>Some necessaries to run the program: </h4>
<p>  -Set up the database under the database folder
  
-Add models with EF Designer from Database (without Pluralize)
  
-After creating models you should add some local variables 
  that I didn't see it necessary on database;</p>
  
    USERS:
    public string UserMail { get; set; }
    public int UserPassword { get; set; }
  
    RESTAURANT:
    public string RestaurantMail { get; set; }
    public int RestaurantPassword { get; set; }
  
    FOOD:
    public int TotalPrice { get; set; }
