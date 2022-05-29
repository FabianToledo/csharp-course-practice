using CoronaStatsAPI.DTO;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace CoronaStatsAPI.Services;

public class ImportDataService : IImportDataService
{
    private readonly HttpClient _httpClient;

    public ImportDataService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<StatesDistrictsDTO>> GetStatesCSVAsync()
    {
        var csvStream = await _httpClient.GetStreamAsync(new Uri("http://www.statistik.at/verzeichnis/reglisten/polbezirke.csv"));

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ";",
            ShouldSkipRecord = content =>
                content.Record[0].ToLower().Contains("politische bezirke") ||
                content.Record[0].ToLower().Contains("erstellt am")
        };

        using var csv = new CsvReader(new StreamReader(csvStream), config);

        var records = csv.GetRecords<StatesDistrictsDTO>().ToList();
        return records;
    }

    public async Task<List<CovidDataDTO>> GetCovidDataCSVAsync()
    {
        var csvStream = await _httpClient.GetStreamAsync(new Uri("https://covid19-dashboard.ages.at/data/CovidFaelle_GKZ.csv"));
        
        var config = new CsvConfiguration(CultureInfo.InvariantCulture) 
        {
            Delimiter = ";" 
        };

        using var csv = new CsvReader(new StreamReader(csvStream), config);

        var records = csv.GetRecords<CovidDataDTO>().ToList();
        return records;

    }

}
