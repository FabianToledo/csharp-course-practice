using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HotelDataLayer.Model;
public class RoomType
{
    public int Id { get; set; }
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;
    [MaxLength(300)]
    public string Description { get; set; } = string.Empty;
    [MaxLength(20)]
    public string Size { get; set; } = string.Empty;
    public bool IsAccessible { get; set; }
    public int QuantityAvailable { get; set; }
    [JsonIgnore]
    public Hotel Hotel { get; set; } = null!;
    public int HotelId { get; set; }
    public RoomPrice RoomPrice { get; set; } = null!;
}
