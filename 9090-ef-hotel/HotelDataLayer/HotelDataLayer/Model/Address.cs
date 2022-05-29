using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HotelDataLayer.Model;
public class Address
{
    public int Id { get; set; }
    [MaxLength(50)]
    public string Street { get; set; } = string.Empty;
    [MaxLength(50)]
    public string ZipCode { get; set; } = string.Empty;
    [MaxLength(50)]
    public string City { get; set; } = string.Empty;
    [JsonIgnore]
    public Hotel Hotel { get; set; } = null!;
    public int HotelId { get; set; }

}
