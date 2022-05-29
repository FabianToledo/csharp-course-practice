using System.ComponentModel.DataAnnotations;

namespace ChuckNorrisIo.Model;
public class ChuckWitz
{
    public int Id { get; set; }
    [MaxLength(40)]
    public string ChuckNorrisId { get; set; } = string.Empty;
    [MaxLength(1024)]
    public string Url { get; set; } = string.Empty;
    public string Witz { get; set; } = string.Empty;
}
