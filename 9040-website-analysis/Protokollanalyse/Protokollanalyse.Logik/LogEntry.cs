
using System.Globalization;

namespace Protokollanalyse.Logik;

public class LogEntry
{
    public string? PhotoUrl { get; set; }
    public DateOnly DownloadDate { get; set; }
    public TimeOnly DownloadTimeOfDay { get; set; }

    public LogEntry(string photoUrl, string date, string time)
    {
        PhotoUrl = photoUrl;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        DownloadDate = DateOnly.Parse(date);
        DownloadTimeOfDay = TimeOnly.Parse(time);
    }

    public static LogEntry FromFileLine(string line)
    {
        var fields = line.Trim().Split('\t');
        return new LogEntry(fields[0], fields[1], fields[2]);
    }
}
