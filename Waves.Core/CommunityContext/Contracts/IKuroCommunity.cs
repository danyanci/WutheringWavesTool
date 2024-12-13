using Waves.Core.Contracts;

namespace Waves.Core.CommunityContext.Contracts;

/// <summary>
/// 库街区社区
/// </summary>
public interface IKuroCommunity
{
    public IHttpClientService HttpClientService { get; }
}
