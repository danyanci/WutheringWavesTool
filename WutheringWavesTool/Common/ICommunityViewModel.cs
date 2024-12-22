using System;
using System.Threading.Tasks;
using Waves.Api.Models.Communitys;

namespace WutheringWavesTool.Common;

public interface ICommunityViewModel : IDisposable
{
    public Task SetDataAsync(GameRoilDataItem item);
}
