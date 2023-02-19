using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using MyBoards.Dto;
using MyBoards.Entities;
using System.Linq.Expressions;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddDbContext<MyBoardsContext>(
    option => option
    //.UseLazyLoadingProxies()
    .UseSqlServer(builder.Configuration.GetConnectionString("MyBoardsConnectionString"))
    );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetService<MyBoardsContext>();

var pendingMigrations = dbContext.Database.GetPendingMigrations();
if (pendingMigrations.Any())
{
    dbContext.Database.Migrate();
}

var users = dbContext.Users.ToList();
if (!users.Any())
{
    var user1 = new User()
    {
        Email = "user1@test.com",
        FullName = "User One",
        Adress = new Adress()
        {
            City = "Warszwa",
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
            Street = "D³uga"
        }
    };

    dbContext.Users.AddRange(user1, user2);

    dbContext.SaveChanges();
}

app.MapGet("pagination", async (MyBoardsContext db) =>
{
    // input
    var filter = "a";
    string sortBy = "FullName"; //"FullName" "Email" null
    bool sortByDescending = false;
    int pageNumber = 1;
    int pageSize = 10;

    var query = db.Users
    .Where(u => filter == null || (u.Email.Contains(filter.ToLower()) || u.FullName.ToLower().Contains(filter.ToLower())));

    var totalCount = query.Count();

    if(sortBy != null)
    {

        var columnsSelector = new Dictionary<string, Expression<Func<User, object>>>
        {
            {nameof(User.Email),user =>user.Email},
            {nameof(User.FullName),user =>user.FullName},
        };
        var sortByExpression = columnsSelector[sortBy];
        query = sortByDescending
        ? query.OrderByDescending(sortByExpression) 
        : query.OrderBy(sortByExpression);

        query.OrderBy(sortByExpression);
    }

   var queryPaginated =  query.Skip(pageSize * (pageNumber - 1))
         .Take(pageSize)
         .ToList();

    var pagedResult = new PagedResult<User>(queryPaginated, totalCount, pageSize, pageNumber);

    return pagedResult;

});


app.MapGet("data", async (MyBoardsContext db) =>
{

    var withAdress = true;

    var users = db.Users
    .First(u => u.Id == Guid.Parse("EBFBD70D-AC83-4D08-CBC6-08DA10AB0E61"));

    if (withAdress)
    {
        var rsult = new { Fullname = users.FullName, Adress = $"{users.Adress.Street} {users.Adress.City}" };
        return rsult;
    }
    return new { Fullname = users.FullName, Adress = "-" };
});

app.MapPost("update", async (MyBoardsContext db) =>
{
    Epic epic = await db.Epic.FirstAsync(epic => epic.Id == 1);

    var rejectedState = await db.WorkItemStates.FirstAsync(a => a.Value == "Rejected");

    epic.State = rejectedState;

    epic.StateId = 1;

    await db.SaveChangesAsync();
    return epic;
});

app.MapPost("create", async (MyBoardsContext db) =>
{
    var adress = new Adress()
    {
        Id = Guid.Parse("b323dd7c-776a-4cf6-a92a-12df154b4a2c"),
        City = "Kraków",
        Country = "Poland",
        Street = "D³uga"
    };

    var user = new User()
    {
        Email = "user@test.com",
        FullName = "Test User",
        Adress = adress,
    };

    db.Users.Add(user);
    await db.SaveChangesAsync();

    return user;

});

app.MapDelete("delete", async (MyBoardsContext db) =>
{
    var user = await db.Users
    .Include(u => u.Comments)
    .FirstAsync(u => u.Id == Guid.Parse("4EBB526D-2196-41E1-CBDA-08DA10AB0E61"));

    db.Users.Remove(user);

    await db.SaveChangesAsync();
});



app.Run();

