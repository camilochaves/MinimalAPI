//LESSON 08 - Authentication and Authorization
//Configured SWAGGER to allow users to authenticate in it

using Microsoft.AspNetCore.Mvc;
using MinimalAPI.Repositories;
using MinimalAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<CustomerRepository>();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(x =>
{
    x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Description="Jwt Authorization Header using JWT bearer scheme",
        Name="CamiloAuth",
        In=Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type=Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
    });
    x.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }}, new List<string>()
        }
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();


builder.Services.AddAuthorization( options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
    .RequireAuthenticatedUser()
    .Build();
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();

//GET
app.MapGet("/", () => "Hello World!");

app.MapGet("/customers", ([FromServices] CustomerRepository crepo) =>
{
    return Results.Ok(crepo.GetAll());
}).AllowAnonymous().Produces<Customer>();

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
app.MapPost("/customers", 
    [AllowAnonymous]
    ([FromServices] CustomerRepository crepo, Customer customer) =>
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



