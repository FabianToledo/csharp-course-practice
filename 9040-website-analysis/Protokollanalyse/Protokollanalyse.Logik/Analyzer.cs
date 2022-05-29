namespace Protokollanalyse.Logik;

public class Analyzer
{
 
    public static IEnumerable<string> SummarizeMonthly(IEnumerable<LogEntry> logEntries, string? photo = null)
    {
        
        var grouped = logEntries
                        .GroupBy(l => new { Month = l.DownloadDate.ToString("yyyy-MM"), l.PhotoUrl })
                        .OrderBy(g => g.Key.Month)
                        .GroupBy(l => l.Key.PhotoUrl)
                        .OrderBy(g => g.Key).AsEnumerable();

        if (photo != null)
            grouped = grouped.Where(l => l.Key == photo);

        foreach(var group in grouped)
        {
            var photoUrl = group.Key;
            yield return $"{photoUrl}:";
            int total = 0;
            foreach (var entries in group)
            {
                var count = entries.Count();
                total += count;
                yield return $"\t{entries.Key.Month}: {count}";
            }
            yield return $"\tTOTAL: {total}";
        }
    }

    public static IEnumerable<string> SummarizeHourly(IEnumerable<LogEntry> logEntries, string? photo = null)
    {

        var grouped = logEntries
                        .GroupBy(l => new { Hour = l.DownloadTimeOfDay.ToString("HH:00"), l.PhotoUrl })
                        .OrderBy(g => g.Key.Hour)
                        .GroupBy(l => l.Key.PhotoUrl)
                        .OrderBy(g => g.Key).AsEnumerable();

        var totalPerPhoto = logEntries.GroupBy(e => e.PhotoUrl).Select(g => new
        {
            PhotoUrl = g.Key,
            Count = g.Count()
        });

        if (photo != null)
            grouped = grouped.Where(l => l.Key == photo);

        foreach (var group in grouped)
        {
            var photoUrl = group.Key;
            yield return $"{photoUrl}:";
            foreach (var entries in group)
            {
                double count = entries.Count();
                var total = totalPerPhoto.FirstOrDefault(a => a.PhotoUrl == group.Key)?.Count;
                yield return $"\t{entries.Key.Hour}: {100*count/total:F} %";
            }
        }
    }

    public static IEnumerable<string> SummarizePhotographers(IEnumerable<LogEntry> logEntries, IEnumerable<Photo> photo)
    {
        return logEntries.GroupBy(e => e.PhotoUrl)
            .Select(g => 
                new 
                {
                    Photographer = photo.FirstOrDefault(p => p.pic == g.Key)?.takenBy ?? "",
                    PhotoUrl = g.Key,
                    Downloads = g.Count()
                })
            .OrderByDescending(a => a.Downloads)
            .Select(a => $"{a.Photographer}: {a.Downloads}")
            ;
    }

}
