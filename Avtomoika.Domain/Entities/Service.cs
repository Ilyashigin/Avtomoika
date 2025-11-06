namespace Avtomoika.Domain.Entities;

public class Service
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public List<Order>? Orders { get; set; } = new();
}