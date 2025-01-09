using ZXing;
using ZXing.Net.Maui;

namespace KLManager;

public partial class MainPage : ContentPage
{
    int count = 0;

    public MainPage()
    {
        InitializeComponent();
        camera.Options = new BarcodeReaderOptions
        {
            Formats = BarcodeFormats.TwoDimensional,
            AutoRotate = true,
            Multiple = false,
        };
    }

    private void camera_BarcodesDetected(
        object sender,
        ZXing.Net.Maui.BarcodeDetectionEventArgs e
    ) { }
}
