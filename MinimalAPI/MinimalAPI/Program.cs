//LESSON 05 - FLUENT API 
// Specified the type of object returned on GET CUSTOMERS and GET CUSTOMERS ASYNC
// Changed initial route to SWAGGER

using Microsoft.AspNetCore.Mvc;
using MinimalAPI.Repositories;
using MinimalAPI.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<CustomerRepository>();
//Added Swagger Config
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//Added Swagger Use
app.UseSwagger();
app.UseSwaggerUI();

//GET
app.MapGet("/", () => "Hello World!");

//GET CUSTOMERS SPECIFYING THE RETURNED TYPE TO SWAGGER - NEW WAY WITH FLUENT API
app.MapGet("/customers", ([FromServices] CustomerRepository crepo) =>
{
    return Results.Ok(crepo.GetAll());
}).Produces<Customer>();

app.MapGet("/customersAsync", async
    ([FromServices] CustomerRepository crepo) =>
{
    await Task.Delay(1000);
    return crepo.GetAll();
}).Produces<List<Customer>>();

app.MapGet("/customer/{id}", //OLD WAY OF USING ATTRIBUTES
    [ProducesResponseType(200,Type =typeof(Customer))]
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



