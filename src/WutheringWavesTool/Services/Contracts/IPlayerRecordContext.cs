using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Waves.Api.Models.Record;

namespace WutheringWavesTool.Services.Contracts;

public interface IPlayerRecordContext : IDisposable
{
    public IDialogManager DialogManager { get; }

    public ITipShow TipShow { get; }

    public IRecordCacheService RecordCacheService { get; }
    public IServiceScope Scope { get; }
    public INavigationService NavigationService { get; }

    public void SetScope(IServiceScope scope);

    public FiveGroupModel FiveGroupModel { get; set; }

    public List<CommunityRoleData> CommunityRoleDatas { get; set; }

    public Task<(string, RecordCacheDetily)> ShowInputRecordAsync(object data);
}
