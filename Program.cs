using LoncotesLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// allows passing datetimes without time zone data 
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// allows our api endpoints to access the database through Entity Framework Core
builder.Services.AddNpgsql<LoncotesLibraryDbContext>(builder.Configuration["LoncotesLibraryDbConnectionString"]);

// Set the JSON serializer options
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/api/materials", (LoncotesLibraryDbContext db, int? materialTypeId, int? genreId) =>
{
    var query = db.Materials
    .Where(m => m.OutOfCirculationSince == null);

    if (materialTypeId.HasValue)
    {
        query = query.Where(m => m.MaterialTypeId == materialTypeId);
    }

    if (genreId.HasValue)
    {
        query = query.Where(m => m.Genre.Id == genreId);
    }

    return query
    .Include(m => m.Genre)
    .Include(m => m.MaterialType)
    .ToList();
});

app.MapGet("/api/materials/{id}", (LoncotesLibraryDbContext db, int id) =>
{
    Material foundMaterial = db.Materials
    .Include(m => m.Genre)
    .Include(m => m.MaterialType)
    .Include(m => m.Checkouts)
    .ThenInclude(c => c.Patron)
    .SingleOrDefault(m => m.Id == id);

    if (foundMaterial == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(foundMaterial);
});

app.MapPost("/api/materials", (LoncotesLibraryDbContext db, Material material) =>
{
    db.Materials.Add(material);
    db.SaveChanges();
    return Results.Created($"/api/materials/{material.Id}", material);
});

// app.MapDelete("/api/materials/{id}", (LoncotesLibraryDbContext db, int id) =>
// {
//     Material materialToDelete = db.Materials.SingleOrDefault(m => m.Id == id);
//     if (materialToDelete == null)
//     {
//         return Results.NotFound();
//     }
//     db.Materials.Remove(materialToDelete);
//     db.SaveChanges();
//     return Results.NoContent();
// });
// supposed to do a soft delete dummy

app.MapPut("/api/materials/{id}", (LoncotesLibraryDbContext db, int id) =>
{
    Material materialToRemoveFromCirculation = db.Materials.SingleOrDefault(m => m.Id == id);
    if (materialToRemoveFromCirculation == null)
    {
        return Results.NotFound();
    }
    DateTime now = DateTime.Now;
    materialToRemoveFromCirculation.OutOfCirculationSince = now;
    db.SaveChanges();
    return Results.NoContent();
});

app.MapGet("/api/materialTypes", (LoncotesLibraryDbContext db) =>
{
    return db.MaterialTypes.ToList();
});

app.MapGet("/api/genres", (LoncotesLibraryDbContext db) =>
{
    return db.Genres.ToList();
});

app.MapGet("/api/patrons", (LoncotesLibraryDbContext db) =>
{
    return db.Patrons.ToList();
});

app.MapGet("api/patrons/{id}", (LoncotesLibraryDbContext db, int id) =>
{
    return db.Patrons
    .Include(p => p.Checkouts)
    .ThenInclude(c => c.Material)
    .ThenInclude(m => m.MaterialType)
    .SingleOrDefault(p => p.Id == id);
});

app.MapPut("/api/patrons/{id}", (LoncotesLibraryDbContext db, int id, Patron patron) =>
{
    Patron patronToUpdate = db.Patrons.SingleOrDefault(p => p.Id == id);
    if (patronToUpdate == null)
    {
        return Results.NotFound();
    }
    if (patron.Address != null)
    {
    patronToUpdate.Address = patron.Address;
    }
    if (patron.Email != null)
    {
    patronToUpdate.Email = patron.Email;
    }

    db.SaveChanges();
    return Results.Ok(patron);
});

app.MapPut("/api/patrons/{id}/deactivate", (LoncotesLibraryDbContext db, int id) =>
{
    Patron patronToDeactivate = db.Patrons.SingleOrDefault(p => p.Id == id);
    if (patronToDeactivate == null)
    {
        return Results.NotFound();
    }
    patronToDeactivate.IsActive = false;

    db.SaveChanges();
    return Results.NoContent();
});

app.MapPost("/api/checkouts", (LoncotesLibraryDbContext db, Checkout checkout) =>
{
    DateTime today = DateTime.Today;
    checkout.CheckoutDate = today;
    db.Checkouts.Add(checkout);
    Material checkedOutMaterial = db.Materials.SingleOrDefault(m => m.Id == checkout.MaterialId);
    if (checkedOutMaterial == null)
    {
        return Results.NotFound();
    }
    checkedOutMaterial.Checkouts.Add(checkout);
    Patron patronCheckingOut = db.Patrons.SingleOrDefault(p => p.Id == checkout.PatronId);
    if (patronCheckingOut == null)
    {
        return Results.NotFound();
    }
    patronCheckingOut.Checkouts.Add(checkout);
    db.SaveChanges();
    return Results.NoContent();
});

app.MapPut("/api/checkouts/return/{id}", (LoncotesLibraryDbContext db, int id) =>
{
    Checkout checkoutToBeReturned = db.Checkouts.SingleOrDefault(c => c.Id == id);
    if(checkoutToBeReturned == null)
    {
        return Results.NotFound();
    }
    DateTime today = DateTime.Today;
    checkoutToBeReturned.ReturnDate = today;
    db.SaveChanges();
    return Results.NoContent();
});

app.MapGet("/api/materials/available", (LoncotesLibraryDbContext db) =>
{
    return db.Materials
    .Where(m => m.OutOfCirculationSince == null)
    .Where(m => m.Checkouts.All(co => co.ReturnDate != null))
    .Include(m => m.Genre)
    .Include(m => m.MaterialType)
    .ToList();
});

app.MapGet("/api/checkouts/overdue", (LoncotesLibraryDbContext db) =>
{
    return db.Checkouts
    .Include(p => p.Patron)
    .Include(co => co.Material)
    .ThenInclude(m => m.MaterialType)
    .Where(co =>
    (DateTime.Today - co.CheckoutDate).Days >
    co.Material.MaterialType.CheckoutDays &&
    co.ReturnDate == null)
    .ToList();
});

app.MapGet("/api/checkouts", (LoncotesLibraryDbContext db) =>
{
    return db.Checkouts
    .Include(p => p.Patron)
    .Include(co => co.Material)
    .ThenInclude(m => m.MaterialType)
    .ToList();
});

app.Run();
