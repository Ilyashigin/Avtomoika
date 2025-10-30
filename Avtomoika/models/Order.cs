namespace Avtomoika.models;

public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public Client? Client { get; set; }
    public int CarId { get; set; }
    public Car? Car { get; set; }
    public int ServiceId { get; set; }
    public Service? Service { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = "Ожидание";
}