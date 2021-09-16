//LESSON 06 - Authentication and Authorization
//Added package Microsoft.AspNetCore.Authentication.JwtBeare
//Configure some endpoints with fluent api RequireAuthorization() and
//the old way using attributes [Authorize]


using Microsoft.AspNetCore.Mvc;
using MinimalAPI.Repositories;
using MinimalAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<CustomerRepository>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//ADDED
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
//ADDED
app.UseAuthentication();
app.UseAuthorization();

//GET
app.MapGet("/", () => "Hello World!");

app.MapGet("/customers", ([FromServices] CustomerRepository crepo) =>
{
    return Results.Ok(crepo.GetAll());
}).RequireAuthorization().Produces<Customer>();

app.MapGet("/customersAsync", async
    ([FromServices] CustomerRepository crepo) =>
{
    await Task.Delay(1000);
    return crepo.GetAll();
}).Produces<List<Customer>>();

app.MapGet("/customer/{id}", //OLD WAY OF USING ATTRIBUTES
    [ProducesResponseType(200,Type =typeof(Customer))]
    [Authorize]
    ([FromServices] CustomerRepository crepo, Guid Id) =>
 {
     var customer = crepo.GetById(Id);
     return customer is not null ?
        Results.Ok(customer) : Results.NotFound();
 });

//POST
app.MapPost("/customers", ([FromServices] CustomerRepository crepo, Customer customer) =>
{
    crepo.Create(customer);
    return Results.Created($"/customers/{customer.Id}", customer);
}).Produces<Customer>();

//UPDATE
app.MapPut("/customers/{id}", ([FromServices] CustomerRepository crepo, Guid Id, Customer newCustomer) =>
{
    var customer = crepo.GetById(Id);
    if (customer is null) return Results.NotFound();
    crepo.Update(newCustomer);
    return Results.Ok(newCustomer);
}).Produces<Customer>();

//DELETE
app.MapDelete("/customers/{id}", ([FromServices] CustomerRepository crepo, Guid Id) =>
{
    crepo.Delete(Id);
    return Results.Ok();
});

app.Run();



