using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<CustomerRepository>();
var app = builder.Build();

//GET
app.MapGet("/", () => "Hello World!");

app.MapGet("/customers", ([FromServices] CustomerRepository crepo) => crepo.GetAll());

app.MapGet("/customersAsync", async ([FromServices] CustomerRepository crepo) =>
{
    await Task.Delay(1000);
    return crepo.GetAll();
});

app.MapGet("/customer/{id}", ([FromServices] CustomerRepository crepo, Guid Id) =>
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
});

//UPDATE
app.MapPut("/customers/{id}", ([FromServices] CustomerRepository crepo, Guid Id, Customer newCustomer) =>
{
    var customer = crepo.GetById(Id);
    if (customer is null) return Results.NotFound();
    crepo.Update(newCustomer);
    return Results.Ok(newCustomer);
});

//DELETE
app.MapDelete("/customers/{id}", ([FromServices] CustomerRepository crepo, Guid Id) =>
{
    crepo.Delete(Id);
    return Results.Ok();
});

app.Run();
record Customer(Guid Id, string FullName);

class CustomerRepository
{
    private Dictionary<Guid, Customer> _customers = new();

    public void Create(Customer customer)
    {
        if (customer is null) return;
        _customers[customer.Id] = customer;
    }

    public Customer GetById(Guid Id)
    {
        var customer = _customers.FirstOrDefault(x => x.Value.Id == Id).Value;
        if (customer is null) return null;
        return customer;
    }

    public List<Customer> GetAll() => _customers.Values.ToList();
    public void Update(Customer customer) => _customers[customer.Id] = customer;
    public void Delete(Guid Id) => _customers.Remove(Id);


}
