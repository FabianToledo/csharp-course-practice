using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace HotelDataLayer.Model;
public class Hotel
{
    public int Id { get; set; }
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    public Address Address { get; set; } = null!;
    public List<Special> Specials { get; set; } = new();
    public List<RoomType> RoomTypes { get; set; } = new();
}

