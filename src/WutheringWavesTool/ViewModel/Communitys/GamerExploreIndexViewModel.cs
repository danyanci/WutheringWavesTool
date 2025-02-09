using System;

namespace WutheringWavesTool.ViewModel.Communitys;

public partial class GamerExploreIndexViewModel : ViewModelBase, IDisposable
{
    private bool disposedValue;
    private GameRoilDataItem? _roilData;

    public IWavesClient WavesClient { get; }
    public ITipShow TipShow { get; }

    [ObservableProperty]
    public partial double TotalProgress { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<DataCenterExploreItem> Explores { get; set; }

    public GamerExploreIndexViewModel(IWavesClient wavesClient, ITipShow tipShow)
    {
        WavesClient = wavesClient;
        TipShow = tipShow;
    }

    internal async Task SetDataAsync(GameRoilDataItem item)
    {
        this._roilData = item;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                if (Explores != null)
                {
                    foreach (var item in this.Explores)
                    {
                        foreach (var item2 in item.Country)
                        {
                            item2.Items.RemoveAll();
                            item2.Items = null;
                        }
                    }
                    this.Explores.RemoveAll();
                }
                this.Explores = null;
                this.Messenger.UnregisterAll(this);
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    [RelayCommand]
    async Task LoadedAsync()
    {
        await RefreshDataAsync();
    }

    private async Task RefreshDataAsync()
    {
        if (Explores != null)
            Explores.RemoveAll();
        if (Explores == null)
            Explores = new();
        if (this._roilData == null)
        {
            TipShow.ShowMessage("玩家数据拉取失败！", Microsoft.UI.Xaml.Controls.Symbol.Clear);
            return;
        }
        var data = await WavesClient.GetGamerExploreIndexDataAsync(this._roilData, this.CTS.Token);
        if (data == null)
        {
            TipShow.ShowMessage("探索数据拉取失败！", Microsoft.UI.Xaml.Controls.Symbol.Clear);
            return;
        }
        foreach (var item in data.ExploreList)
        {
            this.Explores.Add(new(item));
        }
        double total = 0;
        foreach (var progress in Explores)
        {
            total += progress.CountryProgress;
        }
        TotalProgress =  Math.Round(total / Explores.Count);
    }
}
