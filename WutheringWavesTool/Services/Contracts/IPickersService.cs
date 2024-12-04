using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace WutheringWavesTool.Services.Contracts
{
    public interface IPickersService
    {
        public Task<StorageFolder> GetFolderPicker();

        public Task<StorageFile> GetFileOpenPicker(List<string> extention);

        public FileSavePicker GetFileSavePicker();
    }
}
