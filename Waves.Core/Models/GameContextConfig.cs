namespace Waves.Core.Models
{
    public class GameContextConfig
    {
        public string Index_Source { get; set; }

        public string Launcher_Source { get; set; }

        public string LauncherHeader_Source { get; set; }

        public static GameContextConfig Main =>
            new()
            {
                Index_Source =
                    "https://prod-cn-alicdn-gamestarter.kurogame.com/pcstarter/prod/game/G152/10003_Y8xXrXk65DqFHEDgApn3cpK5lfczpFx5/index.json",
                Launcher_Source =
                    "https://prod-cn-alicdn-gamestarter.kurogame.com/pcstarter/prod/starter/10003_Y8xXrXk65DqFHEDgApn3cpK5lfczpFx5/G152/index.json",
                LauncherHeader_Source =
                    "https://prod-cn-alicdn-gamestarter.kurogame.com/pcstarter/prod/game/G152/10003_Y8xXrXk65DqFHEDgApn3cpK5lfczpFx5/social/zh-Hans.json",
            };

        public static GameContextConfig BiliBili =>
            new()
            {
                Index_Source =
                    "https://prod-cn-alicdn-gamestarter.kurogame.com/pcstarter/prod/game/G152/10003_Y8xXrXk65DqFHEDgApn3cpK5lfczpFx5/index.json",
                Launcher_Source =
                    "https://prod-cn-alicdn-gamestarter.kurogame.com/pcstarter/prod/starter/10003_Y8xXrXk65DqFHEDgApn3cpK5lfczpFx5/G152/index.json",
                LauncherHeader_Source =
                    "https://prod-cn-alicdn-gamestarter.kurogame.com/pcstarter/prod/game/G152/10003_Y8xXrXk65DqFHEDgApn3cpK5lfczpFx5/social/zh-Hans.json",
            };

        public static GameContextConfig Global =>
            new()
            {
                Index_Source =
                    "https://prod-cn-alicdn-gamestarter.kurogame.com/pcstarter/prod/game/G152/10003_Y8xXrXk65DqFHEDgApn3cpK5lfczpFx5/index.json",
                Launcher_Source =
                    "https://prod-cn-alicdn-gamestarter.kurogame.com/pcstarter/prod/starter/10003_Y8xXrXk65DqFHEDgApn3cpK5lfczpFx5/G152/index.json",
                LauncherHeader_Source =
                    "https://prod-cn-alicdn-gamestarter.kurogame.com/pcstarter/prod/game/G152/10003_Y8xXrXk65DqFHEDgApn3cpK5lfczpFx5/social/zh-Hans.json",
            };
    }
}
