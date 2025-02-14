namespace WutheringWavesTool.Pages.Dialogs;

public sealed partial class SelectWallpaperDialog : ContentDialog, IDialog
{
    public SelectWallpaperDialog(SelectWallpaperViewModel viewModel)
    {
        this.InitializeComponent();
        ViewModel = viewModel;
    }

    public SelectWallpaperViewModel ViewModel { get; }

    public void SetData(object data) { }

    private void ItemsView_ItemInvoked(ItemsView sender, ItemsViewItemInvokedEventArgs args)
    {
        this.ViewModel.WallpaperService.SetWrallpaper(
            (args.InvokedItem as WallpaperModel)!.FilePath
        );
    }
}
