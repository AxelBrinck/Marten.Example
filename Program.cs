using Marten;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMarten(options =>
{
    options.Connection(builder.Configuration.GetConnectionString("PostgreSQL"));
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/projection1", (IQuerySession session) =>
{
    
});

app.MapGet("/cart", async (IDocumentSession session) =>
{
    var result = session.Events.StartStream(
        new AddedToCart("Pants", 2),
        new UpdatedShippingInformation("42 Street", "12345")
    );

    await session.SaveChangesAsync();

    return "Created";
});

app.Run();

public class Projection1
{
    public Guid Id { get; set; }
    public List<string> Products { get; set; } = new();

    public void Apply(AddedToCart addedToCart)
    {
        Products.Add(addedToCart.Name);
    }
}

public record AddedToCart(string Name, int Quantity);

public record UpdatedShippingInformation(string Address, string PhoneNumber);