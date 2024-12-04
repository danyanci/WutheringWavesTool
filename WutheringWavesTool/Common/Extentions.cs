using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WutheringWavesTool.Common;

public static class Extentions
{
    public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> ts)
    {
        return new ObservableCollection<T>(ts);
    }
}
