using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HotelDataLayer.Model;

public class Special
{
    public int Id { get; set; }
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    [JsonIgnore]
    public List<Hotel> Hotels { get; set; } = new();
}
