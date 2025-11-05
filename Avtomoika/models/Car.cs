using System.Text.Json.Serialization;

namespace Avtomoika.models;

public class Car
{
    public int Id { get; set; }
    public string? Marka { get; set; }
    public string? Model { get; set; }
    public string? Number { get; set; }
    public int ClientId { get; set; }
    public Client? Client { get; set; }
    public List<Order>? Order { get; set; } = new();
}