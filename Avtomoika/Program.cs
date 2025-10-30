using Avtomoika;
using Avtomoika.models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()
    ));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//GET общий
app.MapGet("/api/clients", async (ApplicationContext db) => await db.Clients.ToListAsync());
app.MapGet("/api/cars", async (ApplicationContext db) => await db.Cars.ToListAsync());
app.MapGet("/api/services", async (ApplicationContext db) => await db.Services.ToListAsync());
app.MapGet("/api/orders", async (ApplicationContext db) => await db.Orders.ToListAsync());
//GET по Id
app.MapGet("/api/cars/{id}", async (ApplicationContext db, int id) => await db.Cars.FindAsync(id));
app.MapGet("/api/clients/{id}", async (ApplicationContext db, int id) => await db.Clients.FindAsync(id));
app.MapGet("/api/services/{id}", async (ApplicationContext db, int id) => await db.Services.FindAsync(id));
app.MapGet("/api/orders/{id}", async (ApplicationContext db, int id) => await db.Orders.FindAsync(id));
//Post
app.MapPost("/api/clients", async (Client client, ApplicationContext db) =>
{
    await db.Clients.AddAsync(client);
    await db.SaveChangesAsync();
    return Results.Created($"/api/clients/{client.Id}", client);
});
app.MapPost("/api/cars", async (Car car, ApplicationContext db) =>
{
    await db.Cars.AddAsync(car);
    await db.SaveChangesAsync();
    return Results.Created($"/api/cars/{car.Id}", car);
});
app.MapPost("/api/services", async(Service service, ApplicationContext db)=>
{
    await db.Services.AddAsync(service);
    await db.SaveChangesAsync();
    return Results.Created($"/api/services/{service.Id}", service);
});
app.MapPost("/api/order", async (Order order, ApplicationContext db) =>
{
    await db.Orders.AddAsync(order);
    await db.SaveChangesAsync();
    return Results.Created($"/api/order/{order.Id}", order);
});

//PUT
app.MapPut("/api/clients", async (Client clientData, ApplicationContext db) =>
{
    var client = await db.Clients.FirstOrDefaultAsync(u => u.Id == clientData.Id);
    if (client == null) return Results.NotFound(new { message = "Пользователь не найден" });
    client.Number = clientData.Number;
    client.Email = clientData.Email;
    client.Name = clientData.Name;
    await db.SaveChangesAsync();
    return Results.Json(client);
});
app.MapPut("/api/cars", async (Car carData, ApplicationContext db) =>
{
    var car = await db.Cars.FirstOrDefaultAsync(u => u.Id == carData.Id);
    if (car == null) return Results.NotFound(new { message = "Пользователь не найден" });
    car.Marka = carData.Marka;
    car.Model = carData.Model;
    car.Number = carData.Number;
    car.ClientId = carData.ClientId;
    await db.SaveChangesAsync();
    return Results.Json(car);
});
app.MapPut("/api/services", async (Service serviceData, ApplicationContext db) =>
{
    var service = await db.Services.FirstOrDefaultAsync(u => u.Id == serviceData.Id);
    if (service == null) return Results.NotFound(new { message = "Пользователь не найден" });
    service.Name = serviceData.Name;
    service.Description = serviceData.Description;
    service.Price = serviceData.Price;
    await db.SaveChangesAsync();
    return Results.Json(service);
});
app.MapPut("/api/order", async (Order orderData, ApplicationContext db) =>
{
    var order = await db.Orders.FirstOrDefaultAsync(u => u.Id == orderData.Id);
    if (order == null) return Results.NotFound(new { message = "Пользователь не найден" });
    order.CustomerId = orderData.CustomerId;
    order.CarId = orderData.CarId;
    order.ServiceId = orderData.ServiceId;
    order.OrderDate = orderData.OrderDate;
    order.TotalPrice = orderData.TotalPrice;
    order.Status = orderData.Status;
    await db.SaveChangesAsync();
    return Results.Json(order);
});

//DELETE
app.MapDelete("/api/clients/{id:int}", async (int id, ApplicationContext db) =>
{
    Client? client = await db.Clients.FirstOrDefaultAsync(u => u.Id == id);
    if (client == null) return Results.NotFound(new { message = "Пользователь не найден" });
    db.Clients.Remove(client);
    await db.SaveChangesAsync();
    return Results.NoContent();
});
app.MapDelete("/api/cars/{id:int}", async (int id, ApplicationContext db) =>
{
    Car? car = await db.Cars.FirstOrDefaultAsync(u => u.Id == id);
    if (car == null) return Results.NotFound(new { message = "Пользователь не найден" });
    db.Cars.Remove(car);
    await db.SaveChangesAsync();
    return Results.NoContent();
});
app.MapDelete("/api/services/{id:int}", async (int id, ApplicationContext db) =>
{
    Service? service = await db.Services.FirstOrDefaultAsync(u => u.Id == id);
    if (service == null) return Results.NotFound(new { message = "Пользователь не найден" });
    db.Services.Remove(service);
    await db.SaveChangesAsync();
    return Results.NoContent();
});
app.MapDelete("/api/order/{id:int}", async (int id, ApplicationContext db) =>
{
    Order? order = await db.Orders.FirstOrDefaultAsync(u => u.Id == id);
    if (order == null) return Results.NotFound(new { message = "Пользователь не найден" });
    db.Orders.Remove(order);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
