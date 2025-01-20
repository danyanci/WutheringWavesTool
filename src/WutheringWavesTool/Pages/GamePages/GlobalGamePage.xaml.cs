using WutheringWavesTool.Pages.Bases;

namespace WutheringWavesTool.Pages.GamePages
{
    public sealed partial class GlobalGamePage : GamePageBase, IPage
    {
        public GlobalGamePage()
        {
            this.InitializeComponent();
            this.ViewModel = Instance.Service?.GetRequiredService<GlobalGameViewModel>();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (this.ViewModel != null)
                this.ViewModel.Dispose();
            this.ViewModel = null;
            GC.Collect();
            base.OnNavigatedFrom(e);
        }

        public Type PageType => typeof(GlobalGamePage);

        public GlobalGameViewModel ViewModel { get; private set; }

        private void SelectorBar_SelectionChanged(
            SelectorBar sender,
            SelectorBarSelectionChangedEventArgs args
        )
        {
            this.ViewModel.SelectNews(sender.SelectedItem.Tag.ToString());
        }
    }
}
