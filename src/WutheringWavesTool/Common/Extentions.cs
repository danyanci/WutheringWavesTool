using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace WutheringWavesTool.Common;

public static class Extentions
{
    public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> ts)
    {
        return new ObservableCollection<T>(ts);
    }

    public static void RemoveAll<T>(this ObservableCollection<T> ts)
    {
        if (ts == null || ts.Count == 0)
            return;
        foreach (var item in ts.ToList())
        {
            ts.Remove(item);
        }
    }
}
