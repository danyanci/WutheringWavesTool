using System.Numerics;
using CommunityToolkit.WinUI.Animations;
using WutheringWavesTool.ViewModel.GameViewModels;

namespace WutheringWavesTool.Pages.GamePages;

public sealed partial class MainGamePage : Page, IPage
{
    DispatcherTimer timer = new();

    public MainGamePage()
    {
        this.InitializeComponent();
        this.ViewModel = Instance.Service.GetRequiredService<MainGameViewModel>();

        timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromSeconds(5);
        timer.Tick += Timer_Tick;
        timer.Start();
    }

    bool _isExpander = true;
    int maxCount = 0;
    int currentCount = 0;
    private Vector3 oldOffset;

    private void Timer_Tick(object? sender, object e)
    {
        maxCount = fileView.Items.Count;
        if (currentCount < maxCount - 1)
        {
            currentCount++;
            fileView.SelectedIndex = currentCount;
        }
        else
        {
            currentCount = 0;
            fileView.SelectedIndex = currentCount;
        }
    }

    public MainGameViewModel ViewModel { get; }

    protected override void OnNavigatedFrom(NavigationEventArgs e)
    {
        this.ViewModel.Dispose();
        this.Bindings.StopTracking();
    }

    public Type PageType => typeof(MainGamePage);

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        if (_isExpander)
        {
            oldOffset = leftControl.ActualOffset;
            AnimationSet sets = new();
            sets.Add(
                new OffsetAnimation()
                {
                    To =
                        $"{leftControl.ActualOffset.X - leftControl.ActualWidth + 25},{leftControl.ActualOffset.Y},{leftControl.ActualOffset.Z}",
                }
            );
            sets.Start(this.leftControl);
            _isExpander = false;
            _isExpander = false;
            leftBth.Glyph = "\uE761";
        }
        else
        {
            AnimationSet sets = new();
            sets.Add(new OffsetAnimation() { To = $"{oldOffset.X},{oldOffset.Y},{oldOffset.Z}" });
            sets.Start(this.leftControl);
            _isExpander = true;
            leftBth.Glyph = "\uE760";
        }
    }
}
