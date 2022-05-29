namespace CoronaStatsAPI.DTO;

public class CovidSumDTO
{
    public int DistrictId { get; set; }
    public DateTime Date { get; set; }
    public int PopulationSum { get; set; }
    public int CasesSum { get; set; }
    public int DeathsSum { get; set; }
    public int SevenDaysIncidentsSum { get; set; }
}
