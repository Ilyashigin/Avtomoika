namespace Avtomoika.models;

public class Client
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Number { get; set; }
    public string? Email { get; set; }
    
    public List<Car>? Car { get; set; } = new();
    public List<Order>? Order { get; set; } = new();
}