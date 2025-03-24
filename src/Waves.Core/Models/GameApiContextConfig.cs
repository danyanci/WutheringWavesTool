namespace Waves.Core.Models
{
    public class GameApiContextConfig
    {
        public string Index_Source { get; set; }

        public string Launcher_Source { get; set; }

        public string LauncherHeader_Source { get; set; }

        public static GameApiContextConfig Main =>
            new()
            {
                Index_Source =
                    "https://prod-cn-alicdn-gamestarter.kurogame.com/launcher/launcher/10003_Y8xXrXk65DqFHEDgApn3cpK5lfczpFx5/G152/index.json",
                Launcher_Source =
                    "https://prod-cn-alicdn-gamestarter.kurogame.com/pcstarter/prod/starter/10003_Y8xXrXk65DqFHEDgApn3cpK5lfczpFx5/G152/index.json",
                LauncherHeader_Source =
                    "https://prod-cn-alicdn-gamestarter.kurogame.com/pcstarter/prod/game/G152/10003_Y8xXrXk65DqFHEDgApn3cpK5lfczpFx5/social/zh-Hans.json",
            };

        public static GameApiContextConfig BiliBili =>
            new()
            {
                Index_Source =
                    "https://prod-cn-alicdn-gamestarter.kurogame.com/pcstarter/prod/game/G152/10004_j5GWFuUFlb8N31Wi2uS3ZAVHcb7ZGN7y/index.json",
                Launcher_Source =
                    "https://prod-cn-alicdn-gamestarter.kurogame.com/pcstarter/prod/starter/10004_j5GWFuUFlb8N31Wi2uS3ZAVHcb7ZGN7y/G152/index.json",
                LauncherHeader_Source =
                    "https://prod-cn-alicdn-gamestarter.kurogame.com/pcstarter/prod/game/G152/10004_j5GWFuUFlb8N31Wi2uS3ZAVHcb7ZGN7y/social/zh-Hans.json",
            };

        public static GameApiContextConfig Global =>
            new()
            {
                Index_Source =
                    "https://prod-alicdn-gamestarter.kurogame.com/pcstarter/prod/game/G153/50004_obOHXFrFanqsaIEOmuKroCcbZkQRBC7c/index.json",
                Launcher_Source =
                    "https://prod-alicdn-gamestarter.kurogame.com/pcstarter/prod/starter/50004_obOHXFrFanqsaIEOmuKroCcbZkQRBC7c/G153/index.json",
                LauncherHeader_Source =
                    "https://prod-alicdn-gamestarter.kurogame.com/pcstarter/prod/game/G153/50004_obOHXFrFanqsaIEOmuKroCcbZkQRBC7c/social/zh-Hant.json",
            };
    }
}
