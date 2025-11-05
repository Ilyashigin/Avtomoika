namespace Avtomoika.Aplication.Orders.Dto;

public class OrderCreateDto
{
    public int CustomerId { get; set; }
    public int CarId { get; set; }
    public List<int> ServiceIds { get; set; } = new();
    public DateTime OrderDate { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = "Ожидание";
}