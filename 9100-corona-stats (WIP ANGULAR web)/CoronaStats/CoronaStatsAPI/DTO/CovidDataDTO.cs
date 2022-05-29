using CsvHelper.Configuration.Attributes;

namespace CoronaStatsAPI.DTO;

public class CovidDataDTO
{
    [Name("Bezirk")]
    public string DistrictName { get; set; } = string.Empty;
    [Name("GKZ")]
    public int DistrictCode { get; set; }
    [Name("AnzEinwohner")]
    public int Population { get; set; }
    [Name("Anzahl")]
    public int Cases { get; set; }
    [Name("AnzahlTot")]
    public int TotDeaths { get; set; }
    [Name("AnzahlFaelle7Tage")]
    public int SevenDayCases { get; set; }

}
