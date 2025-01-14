using System.Collections.Generic;
using System.Text.Json;
using Waves.Api.Models;
using Waves.Api.Models.Communitys;
using Waves.Api.Models.Enums;
using Waves.Api.Models.Record;
using Waves.Api.Models.Wrappers;

namespace Waves.Api.Helper;

public static class RecordHelper
{
    public static HttpRequestMessage BuildRequets(RecordRequest recordRequest, CardPoolType type)
    {
        HttpRequestMessage message = new();
        message.RequestUri = new Uri($"https://gmserver-api.aki-game2.com/gacha/record/query");
        message.Method = HttpMethod.Post;
        recordRequest.CardPoolType = (int)type;
        var str = new StringContent(
            JsonSerializer.Serialize(recordRequest, PlayerCardRecordContext.Default.RecordRequest),
            new System.Net.Http.Headers.MediaTypeHeaderValue("application/json", "UTF-8")
        );
        message.Content = str;
        return message;
    }

    public static RecordRequest? GetRecorRequest(string uri)
    {
        try
        {
            RecordRequest request = new();
            var str = uri.Split('?')[1];
            var dic = str.Split('&').Select(x => x.Split('=')).ToDictionary(x => x[0], x => x[1]);
            request.PlayerId = dic["player_id"];
            request.CardPoolId = dic["resources_id"];
            request.ServerId = dic["svr_id"];
            request.Language = dic["lang"];
            request.RecordId = dic["record_id"];
            return request;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static async Task<IEnumerable<RecordCardItemWrapper>?> GetRecordAsync(
        RecordRequest recordRequest,
        CardPoolType type
    )
    {
        try
        {
            var message = BuildRequets(recordRequest, type);
            var client = new HttpClient();
            var response = await client.SendAsync(message);
            response.EnsureSuccessStatusCode();
            var model = JsonSerializer.Deserialize(
                await response.Content.ReadAsStringAsync(),
                PlayerCardRecordContext.Default.PlayerReponse
            );
            List<RecordCardItemWrapper> items = new();
            if (model == null)
                return null;
            if (model != null && model.Code == -1)
                return null;
            if (model != null && model.Code == 0)
            {
                items = model.Data.Select(x => new RecordCardItemWrapper(x)).ToList();
            }
            return items;
        }
        catch (Exception ex)
        {
            return null;
        }
    }

    public static async Task<List<Tuple<RecordCardItemWrapper, int, bool?>>>? FormatStartFiveAsync(
        IEnumerable<RecordCardItemWrapper> source,
        FiveGroupModel? fiveGroup
    )
    {
        List<Tuple<RecordCardItemWrapper, int, bool?>> result = new();
        int count = 1;
        var items = source.Reverse();
        if (fiveGroup == null)
        {
            //无法判断歪不歪
            foreach (var item in items)
            {
                if (item.QualityLevel == 5)
                {
                    result.Add(new(item, count, null));
                    count = 1;
                }
                else
                {
                    count++;
                }
            }
            return result;
        }
        var fileGroup = FormatFiveRoleStar(fiveGroup);

        foreach (var item in items)
        {
            if (item.QualityLevel == 5)
            {
                if (fileGroup.Where(x => x == item.ResourceId).Any())
                {
                    result.Add(new(item, count, false));
                }
                else
                {
                    result.Add(new(item, count, true));
                }
                count = 1;
            }
            else
            {
                count++;
            }
        }
        return result;
    }

    public static List<int> FormatFiveRoleStar(FiveGroupModel model) =>
        model.Data.VersionPools.SelectMany(x => x.UpFiveRoleIds).ToList();

    public static List<int> FormatFiveWeaponeRoleStar(FiveGroupModel model) =>
        model.Data.VersionPools.SelectMany(x => x.UpFiveWeaponIds).ToList();

    public static async Task<FiveGroupModel?> GetFiveGroupAsync()
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                var response = await client.GetAsync(
                    "https://api3.sanyueqi.cn/api/v1/pool/draw_config_infos"
                );
                response.EnsureSuccessStatusCode();
                var model = JsonSerializer.Deserialize(
                    await response.Content.ReadAsStringAsync(),
                    PlayerCardRecordContext.Default.FiveGroupModel
                );
                return model;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    public static async Task<List<CommunityRoleData>?> GetAllRoleAsync()
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                var response = await client.GetAsync("https://mc.appfeng.com/json/avatar.json");
                response.EnsureSuccessStatusCode();
                var model = JsonSerializer.Deserialize(
                    await response.Content.ReadAsStringAsync(),
                    PlayerCardRecordContext.Default.ListCommunityRoleData
                );
                if (model != null)
                    return model;
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
