using Microsoft.EntityFrameworkCore;
using MyBoards.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<MyBoardsContext>(
        option =>option.UseSqlServer(builder.Configuration.GetConnectionString("MyBoardsConnectionString"))    
    );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetService<MyBoardsContext>();

var pendingMigrations = dbContext.Database.GetPendingMigrations();

if(pendingMigrations.Any())
{
    dbContext.Database.Migrate();
}

var users = dbContext.Users.ToList();
if(!users.Any())
{
    var user1 = new User()
    {
        Email = "user1@test.com",
        FullName = "User One",
        Adress = new Adress()
        {
            City = "Waszawa",
            Street = "Szeroka"
        }
    };

    var user2 = new User()
    {
        Email = "user2@test.com",
        FullName = "User Two",
        Adress = new Adress()
        {
            City = "Krakow",
            Street = "Dluga"
        }
    };

    dbContext.Users.AddRange(user1,user2);
    dbContext.SaveChanges();
}

app.MapGet("data", (MyBoardsContext db) =>
{
    var tags = db.Tags.ToList();
    return tags;
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
