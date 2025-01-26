using System.Diagnostics;
using Waves.Core.Models;
using Waves.Core.Models.Enums;

namespace Waves.Core.GameContext;

partial class GameContextBase
{
    public DateTime StartTime { get; private set; }

    public async Task StartLauncheAsync()
    {
        var gameProgram = await this.GameLocalConfig.GetConfigAsync(
            GameLocalSettingName.GameLauncherBassProgram
        );
        if (File.Exists(gameProgram))
        {
            var folder = System.IO.Path.GetDirectoryName(gameProgram);
            if (NowProcess != null)
            {
                NowProcess.Close();
                NowProcess.Dispose();
            }
            this.NowProcess = new Process()
            {
                StartInfo = new ProcessStartInfo(gameProgram)
                {
                    WorkingDirectory = folder,
                    UseShellExecute = true,
                    CreateNoWindow = true,
                    Arguments = this.IsDx11Launche ? "Client -dx11" : "Client -dx12",
                    Verb = "runas",
                },
            };
            NowProcess.EnableRaisingEvents = true;
            NowProcess.Exited += NowProcess_Exited;
            NowProcess.Start();
            this.StartTime = DateTime.Now;
        }
    }

    private void NowProcess_Exited(object? sender, EventArgs e)
    {
        if (NowProcess.HasExited)
        {
            this.IsLaunch = false;
            this.gameContextOutputDelegate?.Invoke(
                this,
                new GameContextOutputArgs() { Type = GameContextActionType.None }
            );
        }
        NowProcess.Exited -= NowProcess_Exited;
        //var time = double.Parse(
        //    await this.GameLocalConfig.GetConfigAsync(GameLocalSettingName.GameTime)
        //);
        //time += (DateTime.Now - StartTime).TotalSeconds;
        //await GameLocalConfig.SaveConfigAsync(GameLocalSettingName.GameTime, time.ToString());
    }
}
