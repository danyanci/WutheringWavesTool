using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Waves.Api.Models;
using Waves.Api.Models.Wrappers;

namespace Project.Test;

[TestClass()]
public class PlayerCardTest
{
    public string CardPoolId = "32bd25a2a19bf4eb212cb11f037b9861";
    public string Language = "zh-Hans";
    public string PlayerId = "104370585";
    public string RecordId = "d1f5255909422e1e1b664b78024c4140";
    public string ServerId = "76402e5b20be2c39f095a152090afddc";

    public HttpRequestMessage BuildRequets()
    {
        HttpRequestMessage message = new();
        message.RequestUri = new Uri($"https://gmserver-api.aki-game2.com/gacha/record/query");
        message.Method = HttpMethod.Post;
        var str = new StringContent(
            JsonSerializer.Serialize(
                new
                {
                    playerId = PlayerId,
                    cardPoolId = CardPoolId,
                    cardPoolType = 1,
                    serverId = ServerId,
                    languageCode = Language,
                    recordId = RecordId,
                }
            ),
            new System.Net.Http.Headers.MediaTypeHeaderValue("application/json", "UTF-8")
        );
        message.Content = str;
        return message;
    }

    [TestMethod]
    public async Task GetPlayerCard()
    {
        using (var http = new HttpClient())
        {
            var reponse = await http.SendAsync(BuildRequets());
            var str = await reponse.Content.ReadAsStringAsync();
            var model = JsonSerializer.Deserialize(
                str,
                PlayerCardRecordContext.Default.PlayerReponse
            );
            if (model != null && model.Code == 0)
            {
                List<RecordCardItemWrapper> items = new();
                items = model.Data.Select(x => new RecordCardItemWrapper(x)).ToList();
            }
        }
    }
}
