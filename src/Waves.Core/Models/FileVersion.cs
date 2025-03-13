using CommunityToolkit.Mvvm.ComponentModel;

namespace Waves.Core.Models;

public class FileVersion : ObservableObject
{
    public string Version { get; set; }

    public string DisplayName { get; set; }

    public string FilePath { get; set; }
    public string? Subtitle { get; internal set; }
}
