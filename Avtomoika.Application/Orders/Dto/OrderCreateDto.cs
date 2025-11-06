namespace Avtomoika.Api.DTOs
{
    public class CreateOrderDto
    {
        public int ClientId { get; set; }
        public int CarId { get; set; }
        public List<int> ServiceIds { get; set; } = new();
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
    }
}