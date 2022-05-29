using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindingDelta.logic;
public class DeltaFinder
{
    static public IEnumerable<string> Deltas(List<Data> oldData, List<Data> newData)
    {
        foreach (Data oldRecord in oldData)
        {
            foreach (var record in DeletedRecords(newData, oldRecord))
            {
                yield return record;
            }
        }
        yield return "";
        foreach (Data newRecord in newData)
        {
             foreach (var record in ChangedRecords(oldData, newRecord))
            {
                yield return record;
            }
        }
        yield return "";
        foreach (Data newRecord in newData)
        {
            foreach (var record in NewRecords(oldData, newRecord))
            {
                yield return record;
            }
        }
    }

    static IEnumerable<string> ChangedRecords(List<Data> oldData, Data newRecord)
    {
        var inBoth = oldData.Where(o => o.book_iban == newRecord.book_iban)
            .Where(o => o.book_title == newRecord.book_title)
            .Where(o => o.year == newRecord.year);

        if (inBoth.Any())
        {
            var changed = inBoth.Where(o => o.genre != newRecord.genre || o.revenue != newRecord.revenue)
                .Select(o => $"~ {o}");
            
            foreach(var changedRecord in changed)
            {
                yield return changedRecord;
            }
        }
    }

    static IEnumerable<string> NewRecords(List<Data> oldData, Data newRecord)
    {
        var inBoth = oldData.Where(o => o.book_iban == newRecord.book_iban)
            .Where(o => o.book_title == newRecord.book_title)
            .Where(o => o.year == newRecord.year);

        if (!inBoth.Any())
        {
            yield return $"+ {newRecord}";
        }
    }

    static IEnumerable<string> DeletedRecords(List<Data> newData, Data oldRecord)
    {
        var inBoth = newData.Where(o => o.book_iban == oldRecord.book_iban)
            .Where(o => o.book_title == oldRecord.book_title)
            .Where(o => o.year == oldRecord.year);

        if (!inBoth.Any())
        {
            yield return $"- {oldRecord}";
        }
    }

}
