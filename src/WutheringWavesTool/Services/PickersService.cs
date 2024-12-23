using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using WutheringWavesTool;
using WutheringWavesTool.Services.Contracts;

namespace WutheringWavesTool.Services;

public class PickersService : IPickersService
{
    public PickersService(IAppContext<App> applicationSetup)
    {
        ApplicationSetup = applicationSetup;
    }

    public IAppContext<App> ApplicationSetup { get; }

    public async Task<StorageFile> GetFileOpenPicker(List<string> extention)
    {
        FileOpenPicker openPicker = new FileOpenPicker();
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(ApplicationSetup.App.MainWindow);
        WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);
        foreach (var item in extention)
        {
            openPicker.FileTypeFilter.Add(item);
        }
        return await openPicker.PickSingleFileAsync();
    }

    public FileSavePicker GetFileSavePicker()
    {
        throw new NotImplementedException();
    }

    public async Task<StorageFolder> GetFolderPicker()
    {
        FolderPicker openPicker = new FolderPicker();
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(ApplicationSetup.App.MainWindow);
        WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);
        return await openPicker.PickSingleFolderAsync();
    }
}
