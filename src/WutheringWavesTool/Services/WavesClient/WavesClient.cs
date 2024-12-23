using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Waves.Api.Models;
using Waves.Api.Models.Communitys;
using Waves.Core.Contracts;
using WavesLauncher.Core.Contracts;
using WavesLauncher.Core.Models;

namespace WutheringWavesTool.Services;

public sealed partial class WavesClient : IWavesClient
{
    public string Token => AppSettings.Token ?? "";

    public long Id => long.Parse(AppSettings.TokenId ?? "0");

    public IHttpClientService HttpClientService { get; }

    public WavesClient(IHttpClientService httpClientService)
    {
        HttpClientService = httpClientService;
        HttpClientService.BuildClient();
    }

    private Dictionary<string, string> GetHeader(bool isNeedToken)
    {
        var dict = new Dictionary<string, string>()
        {
            { "osVersion", "Android" },
            { "devCode", "073A9EFAC18FC50616DD15808DAE719DBCB904B7" },
            { "distinct_id", "96b1567b-b5e6-422f-a1dd-7cb1e58c5db7" },
            { "countryCode", "CN" },
            { "model", "android" },
            { "source", "android" },
            { "lang", "zh-Hans" },
            { "version", "2.2.0" },
            { "versionCode", "2200" },
            { "channelId", "2" },
            { "Accept-Encoding", "gzip" },
            { "User-Agent", "okhttp/3.11.0" },
        };
        if (isNeedToken)
        {
            if (this.Token == null || string.IsNullOrWhiteSpace(Token))
            {
                dict.Add("token", this.Token);
            }
            else
            {
                dict.Add("token", Token);
            }
        }
        return dict;
    }

    private async Task<HttpRequestMessage> BuildLoginRequest(
        string url,
        Dictionary<string, string> headers,
        MediaTypeHeaderValue mediatype,
        Dictionary<string, string> queryValues,
        CancellationToken token = default
    )
    {
        var request = new HttpRequestMessage();
        request.Method = HttpMethod.Post;
        foreach (var item in headers)
        {
            request.Headers.Add(item.Key, item.Value);
        }
        request.RequestUri = new Uri(url);
        var endcod = new FormUrlEncodedContent(queryValues);
        var query = await endcod.ReadAsStringAsync(token);
        request.Content = new StringContent(query, mediatype);
        return request;
    }

    private async Task<HttpRequestMessage> BuildRequest(
        string url,
        HttpMethod method,
        Dictionary<string, string> headers,
        MediaTypeHeaderValue mediatype,
        Dictionary<string, string> queryValues,
        bool IsNeedToken = false,
        CancellationToken token = default
    )
    {
        var request = new HttpRequestMessage();
        request.Method = method;
        foreach (var item in headers)
        {
            request.Headers.Add(item.Key, item.Value);
        }
        request.RequestUri = new Uri(url);
        var endcod = new FormUrlEncodedContent(queryValues);
        var query = await endcod.ReadAsStringAsync(token);
        request.Content = new StringContent(query, mediatype);
        return request;
    }

    public async Task<SignIn?> GetSignInDataAsync(long userId, long roleId)
    {
        var queryData = new Dictionary<string, string>()
        {
            { "gameId", "3" },
            { "serverId", "76402e5b20be2c39f095a152090afddc" },
            { "roleId", roleId.ToString() },
            { "userId", userId.ToString() },
        };
        var header = GetHeader(true);
        var request = await BuildRequest(
            "https://api.kurobbs.com/encourage/signIn/initSignInV2",
            HttpMethod.Post,
            header,
            new("application/x-www-form-urlencoded"),
            queryData,
            true
        );
        var result = await HttpClientService.HttpClient.SendAsync(request);
        var jsonStr = await result.Content.ReadAsStringAsync();
        var sign = JsonSerializer.Deserialize(jsonStr, CommunityContext.Default.SignIn);
        return sign;
    }

    public async Task<SignRecord?> GetSignRecordAsync(string userId, string roldId)
    {
        var header = GetHeader(true);
        var queryData = new Dictionary<string, string>()
        {
            { "gameId", "3" },
            { "serverId", "76402e5b20be2c39f095a152090afddc" },
            { "userId", userId },
            { "roleId", roldId },
            { "reqMonth", DateTime.Now.Month.ToString("D2") },
        };
        var request = await BuildRequest(
            "https://api.kurobbs.com/encourage/signIn/queryRecordV2",
            HttpMethod.Post,
            header,
            new("application/x-www-form-urlencoded"),
            queryData,
            true
        );
        var result = await HttpClientService.HttpClient.SendAsync(request);
        string jsonStr = await result.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize(jsonStr, CommunityContext.Default.SignRecord);
    }

    public async Task<SignInResult?> SignInAsync(string userId, string roleId)
    {
        var header = GetHeader(true);
        var queryData = new Dictionary<string, string>()
        {
            { "gameId", "3" },
            { "serverId", "76402e5b20be2c39f095a152090afddc" },
            { "userId", userId },
            { "roleId", roleId },
            { "reqMonth", DateTime.Now.Month.ToString("D2") },
        };
        var request = await BuildRequest(
            "https://api.kurobbs.com/encourage/signIn/v2",
            HttpMethod.Post,
            header,
            new("application/x-www-form-urlencoded"),
            queryData,
            true
        );
        var result = await HttpClientService.HttpClient.SendAsync(request);
        result.EnsureSuccessStatusCode();
        string jsonStr = await result.Content.ReadAsStringAsync();
        var jsonObj = JsonObject.Parse(jsonStr);
        if (jsonObj["code"]!.GetValue<int>() != 200) { }
        return JsonSerializer.Deserialize(jsonStr, CommunityContext.Default.SignInResult);
    }

    public async Task<AccountMine?> GetWavesMineAsync(long id, CancellationToken token = default)
    {
        var header = GetHeader(true);
        var content = new Dictionary<string, string>() { { "otherUserId", id.ToString() } };
        var request = await BuildRequest(
            "https://api.kurobbs.com/user/mineV2",
            HttpMethod.Post,
            header,
            new MediaTypeHeaderValue("application/x-www-form-urlencoded", "utf-8"),
            content,
            true,
            token
        );
        var result = await HttpClientService.HttpClient.SendAsync(request);
        var jsonStr = await result.Content.ReadAsStringAsync();
        return (AccountMine?)
            JsonSerializer.Deserialize(jsonStr, typeof(AccountMine), CommunityContext.Default);
    }

    public async Task<PlayerReponse?> GetPlayerReponseAsync(PlayerCard card)
    {
        try
        {
            var url = "https://gmserver-api.aki-game2.com/gacha/record/query";
            var content = new StringContent(
                JsonSerializer.Serialize(card, CommunityContext.Default.PlayerCard),
                System.Text.Encoding.UTF8,
                new System.Net.Http.Headers.MediaTypeHeaderValue("application/json")
            );
            var message = new HttpRequestMessage()
            {
                Content = content,
                RequestUri = new Uri(url),
                Method = HttpMethod.Post,
            };
            var reponse = await HttpClientService.HttpClient.SendAsync(message);
            var Jsonstr = await reponse.Content.ReadAsStringAsync();
            return (PlayerReponse?)
                JsonSerializer.Deserialize(
                    await reponse.Content.ReadAsStringAsync(),
                    CommunityContext.Default.PlayerReponse
                );
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public async Task<bool> IsLoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Token) || Id <= 0)
        {
            return false;
        }
        var mine = await GetWavesMineAsync(Id);
        if (mine != null)
        {
            if (mine.Code == 200)
                return true;
        }
        return false;
    }
}
