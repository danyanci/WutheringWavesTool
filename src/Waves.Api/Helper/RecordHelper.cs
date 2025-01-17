using System.Collections.Generic;
using System.Text.Json;
using Waves.Api.Models;
using Waves.Api.Models.Communitys;
using Waves.Api.Models.Enums;
using Waves.Api.Models.Record;
using Waves.Api.Models.Wrappers;
using static System.Formats.Asn1.AsnWriter;

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

    public static async Task<List<RecordCardItemWrapper>?> GetRecordAsync(
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

    public static List<Tuple<RecordCardItemWrapper, int, bool?>>? FormatStartFive(
        IEnumerable<RecordCardItemWrapper> source,
        List<int> ids = null
    )
    {
        List<Tuple<RecordCardItemWrapper, int, bool?>> result = new();
        int count = 1;
        var items = source.Reverse();
        if (ids == null)
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
        foreach (var item in items)
        {
            if (item.QualityLevel == 5)
            {
                if (ids.Where(x => x == item.ResourceId).Any())
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

    public static List<Tuple<RecordCardItemWrapper, int>> FormatRecordFive(
        IEnumerable<RecordCardItemWrapper> source
    )
    {
        List<Tuple<RecordCardItemWrapper, int>> result = new();
        int count = 1;
        //无法判断歪不歪
        foreach (var item in source.Reverse())
        {
            if (item.QualityLevel == 5)
            {
                result.Add(new(item, count));
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

    public static async Task<List<CommunityWeaponData>?> GetAllWeaponAsync()
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                var response = await client.GetAsync("https://mc.appfeng.com/json/weapon.json?");
                response.EnsureSuccessStatusCode();
                var model = JsonSerializer.Deserialize(
                    await response.Content.ReadAsStringAsync(),
                    PlayerCardRecordContext.Default.ListCommunityWeaponData
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

    /// <summary>
    /// 计算小保底歪率
    /// </summary>
    /// <param name="itemWrapper"></param>
    /// <returns></returns>
    public static double GetGuaranteedRange(
        this IEnumerable<Tuple<RecordCardItemWrapper, int, bool?>> itemWrapper
    )
    {
        if (itemWrapper == null || itemWrapper.Count() == 0)
        {
            return 0; // 无数据时返回0
        }

        // 统计小保底次数和歪的次数
        int totalSmallGuarantees = 0;
        int totalSmallGuaranteeFails = 0;

        foreach (var item in itemWrapper)
        {
            var isOffBanner = item.Item3; // 获取是否歪的标志
            if (isOffBanner.HasValue) // 仅统计非null的项
            {
                totalSmallGuarantees++;
                if (isOffBanner.Value) // 记录歪的次数
                {
                    totalSmallGuaranteeFails++;
                }
            }
        }

        // 如果没有有效的小保底记录，歪率为0
        if (totalSmallGuarantees == 0)
        {
            return 0;
        }

        // 计算小保底歪率并返回
        return (double)totalSmallGuaranteeFails / totalSmallGuarantees * 100;
    }

    public static double CalculateAvg(this IEnumerable<Tuple<RecordCardItemWrapper, int>> value) =>
        value.Average(x => x.Item2);

    /// <summary>
    /// 计算此样本的分数
    /// </summary>
    /// <param name="guaranteedRange">小保底歪率</param>
    /// <param name="roleAAvg">活动角色平均抽数</param>
    /// <param name="weaponAAvg">活动武器平均抽数</param>
    /// <param name="roleIAvg">常驻角色平均抽数</param>
    /// <param name="weaponIAvg">常驻武器平均抽数</param>
    /// <returns></returns>
    public static double Score(
        double guaranteedRange,
        double roleAAvg,
        double weaponAAvg,
        double resident
    )
    {
        double weight1 = 0.40;
        double weight2 = 0.2;
        double weight3 = 0.2;
        double weight4 = 0.2;
        double minScore1 = 0;
        double maxScore1 = 100;
        double minScore2 = 0;
        double maxScore2 = 80;
        double minScore3 = 0;
        double maxScore3 = 80;
        double minScore4 = 0;
        double maxScore4 = 80;
        double weightedScore1 =
            (1 - (guaranteedRange - minScore1) / (maxScore1 - minScore1)) * weight1;
        double weightedScore2 = (1 - (roleAAvg - minScore2) / (maxScore2 - minScore2)) * weight2;
        double weightedScore3 = (1 - (weaponAAvg - minScore3) / (maxScore3 - minScore3)) * weight3;
        double weightedScore4 = (1 - (resident - minScore4) / (maxScore4 - minScore4)) * weight4;
        double totalScore =
            (weightedScore1 + weightedScore2 + weightedScore3 + weightedScore4) * 100;
        return totalScore;
    }
}
