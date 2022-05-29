using CsvHelper.Configuration.Attributes;

namespace CoronaStatsAPI.DTO;

public class StatesDistrictsDTO
{
    [Name("Bundeslandkennziffer")]
    public int Id { get; set; }
    [Name("Bundesland")]
    public string StateName { get; set; } = string.Empty;
    [Name("Kennziffer pol. Bezirk")]
    public int DistrictCode { get; set; }
    [Name("Politischer Bezirk")]
    public string DistrictName { get; set; } = string.Empty;
    [Name("Politischer Bez. Code")]
    public int WienCode { get; set; }

}
