namespace WutheringWavesTool.Common;

public static class Extentions
{
    public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> ts)
    {
        if (ts == null)
            return new ObservableCollection<T>();
        return new ObservableCollection<T>(ts);
    }

    public static CardItemObservableCollection<T> ToCardItemObservableCollection<T>(
        this IEnumerable<T> ts
    )
    {
        return new CardItemObservableCollection<T>(ts);
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
