using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Waves.Api.Models.Record;
using WutheringWavesTool.Models.Args;

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

    public Task<CreateRecordArgs?> ShowInputRecordAsync(object data);
}
