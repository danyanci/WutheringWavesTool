using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Resources;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Waves.Api.Models;
using Waves.Core.Contracts;
using Waves.Core.Models;

namespace Waves.Core.GameContext;

public abstract partial class GameContextBase : IGameContext
{
    public GameContextBase(GameApiContextConfig config, string contextName)
    {
        Config = config;
        ContextName = contextName;
    }

    public virtual void Init()
    {
        this.HttpClientService.BuildClient(ContextName);
        Directory.CreateDirectory(GamerConfigPath);
        this.GameLocalConfig = new GameLocalConfig();
        GameLocalConfig.SettingPath = GamerConfigPath + "\\Settings.db";
    }

    public IHttpClientService HttpClientService { get; set; }
    public GameApiContextConfig Config { get; private set; }
    public string ContextName { get; }
    public string GamerConfigPath { get; set; }

    public bool IsNext
    {
        get
        {
            if (!Directory.Exists(GameContextFactory.GameBassPath))
            {
                return false;
            }
            if (!File.Exists(GamerConfigPath + "\\Settings.json"))
            {
                return false;
            }
            return true;
        }
    }

    public GameLocalConfig GameLocalConfig { get; private set; }

    public bool IsLaunch { get; private set; }
    public WavesIndex WavesIndex { get; private set; }
    public CdnList? Cdn { get; private set; }
    public GameResource Resources { get; private set; }

    public async Task<GameContextStatus> GetGameStausAsync(CancellationToken token = default)
    {
        GameContextStatus status = new();
        var gamePath = await this.GameLocalConfig.GetConfigAsync(
            GameLocalSettingName.GameLauncherBassFolder
        );
        if (!string.IsNullOrWhiteSpace(gamePath) || Directory.Exists(gamePath))
        {
            status.IsLauncheExists = true;
        }
        return status;
    }

    public void StartVerifyGame(string exefile)
    {
        Task.Run(async () =>
        {
            try
            {
                var folder = System.IO.Path.GetDirectoryName(exefile);
                if (folder == null)
                    return;
                var launcherIndex = await GetGameIndexAsync();
                if (launcherIndex == null)
                    return;
                this.WavesIndex = launcherIndex;
                this.Cdn = launcherIndex
                    .Default.CdnList.OrderByDescending(p => p.P)
                    .LastOrDefault();
                var resourceUrl = Cdn.Url + launcherIndex.Default.Resources;
                this.Resources = await this.GetGameResourceAsync(resourceUrl);
                this.gameContextOutputDelegate?.Invoke(
                    null,
                    new GameContextOutputArgs()
                    {
                        Type = Models.Enums.GameContextActionType.Verify,
                        CurrentFile = 0,
                        MaxFile = Resources.Resource.Count,
                        Progress = 0.0,
                    }
                );
                List<Resource> resources = new();
                for (int i = 0; i < Resources.Resource.Count; i++)
                {
                    var file = folder + this.Resources.Resource[i].Dest.Replace('/', '\\');
                    if (!File.Exists(file))
                    {
                        resources.Add(this.Resources.Resource[i]);
                        double exis = (((double)i + 1) / this.Resources.Resource.Count) * 100;
                        this.gameContextOutputDelegate?.Invoke(
                            this,
                            new GameContextOutputArgs()
                            {
                                Type = Models.Enums.GameContextActionType.Verify,
                                CurrentFile = i + 1,
                                MaxFile = Resources.Resource.Count,
                                Progress = Math.Round(exis, 2),
                            }
                        );
                        continue;
                    }
                    var md5String = GetFileMD5(file);
                    if (md5String != this.Resources.Resource[i].Md5)
                    {
                        resources.Add(this.Resources.Resource[i]);
                        continue;
                    }

                    double value = (((double)i + 1) / this.Resources.Resource.Count) * 100;
                    this.gameContextOutputDelegate?.Invoke(
                        this,
                        new GameContextOutputArgs()
                        {
                            Type = Models.Enums.GameContextActionType.Verify,
                            CurrentFile = i + 1,
                            MaxFile = Resources.Resource.Count,
                            Progress = Math.Round(value, 2),
                        }
                    );
                }
                if (resources.Count > 0)
                {
                    //进入下载
                    return;
                }
                await this.GameLocalConfig.SaveConfigAsync(
                    GameLocalSettingName.GameLauncherBassFolder,
                    folder
                );
                await this.GameLocalConfig.SaveConfigAsync(
                    GameLocalSettingName.GameLauncherBassProgram,
                    exefile
                );
                this.IsLaunch = true;

                this.gameContextOutputDelegate?.Invoke(
                    this,
                    new GameContextOutputArgs() { Type = Models.Enums.GameContextActionType.None }
                );
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        });
    }

    public string GetFileMD5(string file)
    {
        string md5String = "";
        using (MD5 md5 = MD5.Create())
        {
            using (var fs = File.OpenRead(file))
            {
                byte[] hashValue = md5.ComputeHash(fs);
                StringBuilder hex = new(hashValue.Length * 2);
                foreach (byte b in hashValue)
                {
                    hex.AppendFormat("{0:x2}", b);
                }
                md5String = hex.ToString();
            }
        }
        return md5String;
    }

    public async Task<WavesIndex> GetGameIndexAsync(CancellationToken token = default)
    {
        var result = await HttpClientService.GameDownloadClient.GetAsync(this.Config.Index_Source);
        var launcherIndex = JsonSerializer.Deserialize<WavesIndex>(
            await result.Content.ReadAsStringAsync(),
            WavesIndexContext.Default.WavesIndex
        );
        return launcherIndex;
    }

    public async Task<GameResource> GetGameResourceAsync(
        string resourceUrl,
        CancellationToken token = default
    )
    {
        var result = await HttpClientService.GameDownloadClient.GetAsync(resourceUrl);
        var launcherIndex = JsonSerializer.Deserialize<GameResource>(
            await result.Content.ReadAsStringAsync(),
            GameResourceContext.Default.GameResource
        );
        return launcherIndex;
    }
}
