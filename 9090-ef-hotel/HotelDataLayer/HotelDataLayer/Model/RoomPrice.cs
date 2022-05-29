using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace HotelDataLayer.Model;
public class RoomPrice
{
    public int Id { get; set; }
    [Precision(10, 2)]
    public decimal Price { get; set; }
    public DateTimeOffset? DateFrom { get; set; }
    public DateTimeOffset? DateTo { get; set; }
    [JsonIgnore]
    public RoomType RoomType { get; set; } = null!;
    public int RoomTypeId { get; set; }

}