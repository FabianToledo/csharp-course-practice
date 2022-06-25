using System.ComponentModel.DataAnnotations;

namespace AadFriendsApi.Model;

public class Friend
{
    public int Id { get; set; }

    [MaxLength(80)]
    public string AadId { get; set; } = null!;
}
