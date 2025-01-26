using System;
using System.Collections.ObjectModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace GreenPlayer.Controls;

public partial class MonthItem : Control
{
    public MonthItem(int year, int month)
    {
        this.DefaultStyleKey = nameof(MonthItem);
        Year = year;
        Month = month;
    }

    protected override void OnApplyTemplate()
    {
        Update();
    }

    private void Update()
    {
        ObservableCollection<int> greenItems = new();
        var days = DateTime.DaysInMonth(Year, Month);
        for (int i = 0; i < days; i++)
        {
            greenItems.Add(5);
        }
    }

    public object ItemSource
    {
        get { return (object)GetValue(ItemSourceProperty); }
        set { SetValue(ItemSourceProperty, value); }
    }

    public int Year { get; }
    public int Month { get; }

    public static readonly DependencyProperty ItemSourceProperty = DependencyProperty.Register(
        "ItemSource",
        typeof(object),
        typeof(MonthItem),
        new PropertyMetadata(null)
    );
}
