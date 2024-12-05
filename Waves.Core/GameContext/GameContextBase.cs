using System;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Resources;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using NetTaste;
using Waves.Api.Models;
using Waves.Core.Common;
using Waves.Core.Contracts;
using Waves.Core.Models;

namespace Waves.Core.GameContext;

public abstract partial class GameContextBase : IGameContext
{
    private bool isLimtSpeed;

    public GameContextBase(GameApiContextConfig config, string contextName)
    {
        Config = config;
        ContextName = contextName;
    }

    public virtual async Task InitAsync()
    {
        this.HttpClientService.BuildClient(ContextName);
        Directory.CreateDirectory(GamerConfigPath);
        this.GameLocalConfig = new GameLocalConfig();
        GameLocalConfig.SettingPath = GamerConfigPath + "\\Settings.db";
        var selectFolder = await this.GameLocalConfig.GetConfigAsync(
            GameLocalSettingName.GameLauncherBassFolder
        );
        if (!(selectFolder == null || string.IsNullOrWhiteSpace(selectFolder)))
        {
            this.GameContextDownloadCahce = GameContextFactory.GetGameContextDownloadCache(
                selectFolder
            );
        }
        else
        {
            GameContextDownloadCahce = null;
        }
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
            if (!File.Exists(GamerConfigPath + "\\Settings.db"))
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
    public string DownloadBaseUrl { get; private set; }
    public GameResource Resources { get; private set; }
    public bool IsLimitSpeed
    {
        get => isLimtSpeed;
        set
        {
            this.Lock = new RateLimiter(SpeedValue * 1024 * 1024);
            this.isLimtSpeed = value;
        }
    }

    public int SpeedValue { get; private set; }
    public RateLimiter Lock { get; private set; }
    public bool IsVerify { get; private set; }
    public bool IsDownload { get; private set; }
    public bool IsClear { get; private set; }

    public IGameContextDownloadCache GameContextDownloadCahce { get; private set; }

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
        status.IsVerify = this.IsVerify;
        status.IsDownload = this.IsDownload;
        return status;
    }

    public void StartVerifyGame(string exefile)
    {
        Task.Run(async () =>
        {
            try
            {
                this.IsVerify = true;
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
                this.DownloadBaseUrl = Cdn.Url + WavesIndex.Default.ResourcesBasePath;
                var resourceUrl = Cdn.Url + launcherIndex.Default.Resources;
                this.Resources = await this.GetGameResourceAsync(resourceUrl);
                this.gameContextOutputDelegate?.Invoke(
                    this,
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
                    this.IsVerify = true;
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
                    this.IsVerify = false;
                    //进入下载
                    await DownloadGameFiles(folder, resources);
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

    public void StartDownloadGame(string folder)
    {
        Task.Run(async () =>
        {
            this.IsDownload = true;
            this.GameContextDownloadCahce = GameContextFactory.GetGameContextDownloadCache(folder);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            await GameLocalConfig.SaveConfigAsync(
                GameLocalSettingName.GameLauncherBassFolder,
                folder
            );
            var launcherIndex = await GetGameIndexAsync();
            if (launcherIndex == null)
                return;
            this.WavesIndex = launcherIndex;
            this.Cdn = launcherIndex.Default.CdnList.OrderByDescending(p => p.P).LastOrDefault();
            this.DownloadBaseUrl = Cdn.Url + WavesIndex.Default.ResourcesBasePath;
            var resourceUrl = Cdn.Url + launcherIndex.Default.Resources;
            this.Resources = await this.GetGameResourceAsync(resourceUrl);
            var maxSize = Resources.Resource.Sum(x => x.Size);
            this.gameContextOutputDelegate?.Invoke(
                this,
                new GameContextOutputArgs()
                {
                    Type = Models.Enums.GameContextActionType.Download,
                    CurrentFile = 0,
                    MaxFile = Resources.Resource.Count,
                    Progress = 0.0,
                    CurrentSize = 0,
                    Speed = 0,
                    SpeedString = "0MB/s",
                    MaxSize = maxSize,
                }
            );
            await this.GameContextDownloadCahce.WriteCacheAsync(
                new()
                {
                    NowSize = 0.0,
                    Progress = 0.0,
                    TotalSize = maxSize,
                }
            );
            await DownloadGameFiles(folder, Resources.Resource);
        });
    }

    /// <summary>
    /// 下载游戏文件
    /// </summary>
    /// <param name="folder">基础目录</param>
    /// <param name="resources">游戏资源</param>
    /// <returns></returns>
    private async Task DownloadGameFiles(string folder, List<Resource> resources)
    {
        this.IsDownload = true;
        this.gameContextOutputDelegate?.Invoke(
            this,
            new GameContextOutputArgs()
            {
                Type = Models.Enums.GameContextActionType.Download,
                CurrentFile = 0,
                MaxFile = resources.Count,
                Progress = 0.0,
            }
        );
        var list = resources.OrderByDescending(x => x.Size).ToArray();
        Array.Reverse(list);
        double maxSize = list.Sum(x => x.Size);
        var totalBytesRead = 0L;
        this.gameContextOutputDelegate?.Invoke(
            this,
            new GameContextOutputArgs()
            {
                Type = Models.Enums.GameContextActionType.Verify,
                CurrentFile = 0,
                MaxFile = resources.Count,
                Progress = 0.0,
                Speed = 0,
                SpeedString = "0MB/s",
                CurrentSize = totalBytesRead,
                MaxSize = (long)maxSize,
            }
        );
        for (global::System.Int32 i = 0; i < list.Length; i++)
        {
            this.IsDownload = true;
            var cachefile = folder + "\\DownloadCache" + list[i].Dest.Replace('/', '\\');
            var file = folder + list[i].Dest.Replace('/', '\\');
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(cachefile)!);
            var url = DownloadBaseUrl + list[i].Dest;
            using (
                var response = await HttpClientService.GameDownloadClient.GetAsync(
                    url,
                    HttpCompletionOption.ResponseHeadersRead
                )
            )
            {
                response.EnsureSuccessStatusCode();
                using (var responseStream = await response.Content.ReadAsStreamAsync())
                using (
                    var fileStream = new FileStream(
                        cachefile,
                        FileMode.Create,
                        FileAccess.Write,
                        FileShare.None
                    )
                )
                {
                    var buffer = new byte[819200]; // 80KB 缓冲区
                    int bytesRead;
                    var startTime = DateTime.UtcNow;
                    var lastReportTime = DateTime.UtcNow;
                    var bytesSinceLastReport = 0L;
                    while (
                        (
                            bytesRead = await MaybeLimitAndReadAsync(
                                responseStream,
                                fileStream,
                                buffer,
                                IsLimitSpeed,
                                Lock
                            )
                        ) > 0
                    )
                    {
                        totalBytesRead += bytesRead;
                        bytesSinceLastReport += bytesRead;
                        var currentTime = DateTime.UtcNow;
                        var reportInterval = TimeSpan.FromSeconds(1);
                        if (currentTime - lastReportTime >= reportInterval)
                        {
                            var speed =
                                bytesSinceLastReport / (currentTime - lastReportTime).TotalSeconds;
                            var progresssValue = Math.Round(
                                Convert.ToDouble((double)totalBytesRead / maxSize) * 100,
                                2
                            );
                            await this.GameContextDownloadCahce.WriteCacheAsync(
                                new()
                                {
                                    NowSize = totalBytesRead,
                                    Progress = progresssValue,
                                    TotalSize = maxSize,
                                }
                            );
                            lastReportTime = currentTime;
                            bytesSinceLastReport = 0;
                            this.gameContextOutputDelegate?.Invoke(
                                this,
                                new GameContextOutputArgs()
                                {
                                    Type = Models.Enums.GameContextActionType.Download,
                                    CurrentSize = totalBytesRead,
                                    MaxSize = maxSize,
                                    Speed = speed / 1024 / 1024,
                                    SpeedString = $"{speed / 1024 / 1024:F2} MB/s",
                                    CurrentFile = i,
                                    MaxFile = list.Length,
                                    Progress = progresssValue,
                                }
                            );
                        }
                    }
                    var currentTime2 = DateTime.UtcNow;
                    var speed2 =
                        bytesSinceLastReport / (currentTime2 - lastReportTime).TotalSeconds;
                    var progresssValue2 = Math.Round(
                        Convert.ToDouble((double)totalBytesRead / maxSize) * 100,
                        2
                    );
                    await this.GameContextDownloadCahce.WriteCacheAsync(
                        new()
                        {
                            NowSize = totalBytesRead,
                            Progress = progresssValue2,
                            TotalSize = maxSize,
                        }
                    );
                    this.gameContextOutputDelegate?.Invoke(
                        this,
                        new GameContextOutputArgs()
                        {
                            Type = Models.Enums.GameContextActionType.Download,
                            CurrentSize = totalBytesRead,
                            MaxSize = maxSize,
                            Speed = speed2 / 1024 / 1024,
                            SpeedString = $"{speed2 / 1024 / 1024:F2} MB/s",
                            CurrentFile = i,
                            MaxFile = list.Length,
                            Progress = progresssValue2,
                        }
                    );
                }
            }
        }
        this.gameContextOutputDelegate?.Invoke(
            this,
            new GameContextOutputArgs() { Type = Models.Enums.GameContextActionType.None }
        );
        this.IsDownload = false;
        ClearGameCache(folder, this.Resources.Resource);
        this.IsClear = true;
    }

    private void ClearGameCache(string folder, List<Resource> resources)
    {
        List<Resource> clearResource = new();
        this.gameContextOutputDelegate?.Invoke(
            this,
            new()
            {
                Type = Models.Enums.GameContextActionType.Clear,
                CurrentFile = 0,
                MaxFile = resources.Count,
                Progress = 0,
                Speed = 0.0,
                CurrentSize = 0.0,
                MaxSize = 0.0,
                SpeedString = "",
            }
        );
        for (int i = 0; i < resources.Count; i++)
        {
            var cachefile = folder + "DownloadCache\\" + resources[i].Dest.Replace('/', '\\');
            var file = folder + resources[i].Dest.Replace('/', '\\');
            if (!File.Exists(cachefile))
            {
                clearResource.Add(resources[i]);
                continue;
            }
            var hash = this.GetFileMD5(cachefile);
            if (hash != null && hash == resources[i].Md5)
            {
                File.Move(cachefile, file);
                var progress = Math.Round(Convert.ToDouble((double)i / resources.Count) * 100, 2);
                this.gameContextOutputDelegate?.Invoke(
                    this,
                    new()
                    {
                        Type = Models.Enums.GameContextActionType.Clear,
                        CurrentFile = i + 1,
                        MaxFile = resources.Count,
                        Progress = progress,
                        Speed = 0.0,
                        CurrentSize = 0.0,
                        MaxSize = 0.0,
                        SpeedString = "",
                    }
                );
            }
        }
        this.gameContextOutputDelegate?.Invoke(
            this,
            new()
            {
                Type = Models.Enums.GameContextActionType.None,
                CurrentFile = 0,
                MaxFile = resources.Count,
                Progress = 0.0,
                Speed = 0.0,
                CurrentSize = 0.0,
                MaxSize = 0.0,
                SpeedString = "",
            }
        );
    }

    async Task<int> MaybeLimitAndReadAsync(
        Stream responseStream,
        FileStream fileStream,
        byte[] buffer,
        bool isLimitSpeed,
        RateLimiter rateLimiter
    )
    {
        int bytesRead = await responseStream.ReadAsync(buffer, 0, buffer.Length);
        if (isLimitSpeed)
        {
            await rateLimiter.ConsumeAsync(bytesRead);
        }
        await fileStream.WriteAsync(buffer, 0, bytesRead);
        return bytesRead;
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
