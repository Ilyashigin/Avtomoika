namespace Avtomoika.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    
    public int ClientId { get; set; }
    public Client? Client { get; set; }

    public int CarId { get; set; }
    public Car? Car { get; set; }

    public List<Service>? Services { get; set; } = new();
    public DateTime OrderDate { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = "Ожидание";
}
