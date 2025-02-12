using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using SqlSugar;
using Waves.Api.Models;
using Waves.Core.Common;
using Waves.Core.Contracts;
using Waves.Core.Models;
using Waves.Core.Models.Enums;

namespace Waves.Core.GameContext;

public abstract partial class GameContextBase : IGameContext
{
    private bool isLimtSpeed;
    private CancellationTokenSource _downloadCTS;
    private CancellationTokenSource _prodDowloadCTS;
    private CancellationTokenSource _clearCTS;
    private CancellationTokenSource verifyCts;
    private CancellationTokenSource _installProdCTS;

    internal GameContextBase(GameApiContextConfig config, string contextName)
    {
        Config = config;
        ContextName = contextName;
    }

    public virtual async Task InitAsync()
    {
        this.HttpClientService.BuildClient();
        Directory.CreateDirectory(GamerConfigPath);
        this.GameLocalConfig = new GameLocalConfig();
        GameLocalConfig.SettingPath = GamerConfigPath + "\\Settings.db";
        await InitSettingAsync();
    }

    private async Task InitSettingAsync()
    {
        var config = await this.ReadContextConfigAsync();
        if (config.LimitSpeed > 0)
        {
            this.SpeedValue = config.LimitSpeed;
            this.IsLimitSpeed = true;
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

    public string ProdDownloadBaseUrl { get; private set; }
    public List<Resource> Resources { get; private set; }
    public bool IsDx11Launche { get; private set; }

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

    /// <summary>
    /// 正常限速
    /// </summary>
    public RateLimiter Lock { get; private set; }

    /// <summary>
    /// 预下载限速
    /// </summary>
    public RateLimiter ProdLock { get; private set; }
    public bool IsVerify { get; private set; }
    public bool IsDownload { get; private set; }
    public bool IsClear { get; private set; }

    public bool InstallProd { get; private set; }

    public bool IsPause { get; private set; }

    public IGameContextDownloadCache GameContextDownloadCahce { get; private set; }
    public Process NowProcess { get; private set; }

    public async Task<GameContextStatus> GetGameStatusAsync(CancellationToken token = default)
    {
        GameContextStatus status = new();
        var gamePath = await this.GameLocalConfig.GetConfigAsync(
            GameLocalSettingName.GameLauncherBassFolder
        );
        var programPath = await this.GameLocalConfig.GetConfigAsync(
            GameLocalSettingName.GameLauncherBassProgram
        );
        var gameversion = await this.GameLocalConfig.GetConfigAsync(
            GameLocalSettingName.LocalGameResourceVersion
        );
        if (
            (!string.IsNullOrWhiteSpace(gamePath) || Directory.Exists(gamePath))
            && (!string.IsNullOrWhiteSpace(gamePath) || Directory.Exists(gamePath))
        )
        {
            if (
                (!string.IsNullOrWhiteSpace(programPath) || File.Exists(programPath))
                && (!string.IsNullOrWhiteSpace(gameversion))
            )
            {
                status.IsDownloadComplete = true;
                status.IsSelectDownloadFolder = true;
            }
            else
            {
                status.IsDownloadComplete = false;
                status.IsSelectDownloadFolder = true;
            }
        }
        status.IsVerify = this.IsVerify;
        status.IsDownload = this.IsDownload;
        status.IsClear = this.IsClear;
        status.IsProdDownloading = this.IsProdDownload;
        status.IsProdInstalling = this.InstallProd;
        if (this.DownloadBaseUrl != null)
        {
            var result = await NetworkCheck.PingAsync(DownloadBaseUrl);
            if (result == null)
                status.ConnectNetwork = false;
            else if (result.Status == System.Net.NetworkInformation.IPStatus.Success)
                status.ConnectNetwork = true;
            else
                status.ConnectNetwork = false;
        }
        else
        {
            var baiduResult = await NetworkCheck.PingAsync("https://baidu.com");
            if (baiduResult == null)
                status.ConnectNetwork = false;
            else if (baiduResult.Status == System.Net.NetworkInformation.IPStatus.Success)
                status.ConnectNetwork = true;
            else
                status.ConnectNetwork = false;
        }
        try
        {
            if (this.NowProcess != null)
            {
                if (NowProcess.HasExited)
                    status.IsLauncherGame = false;
                else
                    status.IsLauncherGame = true;
            }
            else
            {
                status.IsLauncherGame = false;
            }
        }
        catch (Exception)
        {
            status.IsLauncherGame = false;
        }
        return status;
    }

    public void StartVerifyGame(string folder)
    {
        Task.Run(async () =>
        {
            try
            {
                if (!Directory.Exists(folder))
                {
                    this.gameContextOutputDelegate?.Invoke(
                        this,
                        new GameContextOutputArgs()
                        {
                            Type = GameContextActionType.Error,
                            ErrorMessage = "目录不存在！",
                        }
                    );
                    return;
                }

                this.verifyCts = new CancellationTokenSource();
                this.IsVerify = true;
                var launcherIndex = await GetGameIndexAsync();
                if (launcherIndex == null)
                    return;
                this.WavesIndex = launcherIndex;
                this.Cdn = launcherIndex
                    .Default.CdnList.OrderByDescending(p => p.P)
                    .LastOrDefault();
                this.DownloadBaseUrl = Cdn.Url + WavesIndex.Default.ResourcesBasePath;
                var resourceUrl = Cdn.Url + launcherIndex.Default.Resources;
                this.Resources = (await this.GetGameResourceAsync(resourceUrl)).Resource;
                this.gameContextOutputDelegate?.Invoke(
                    this,
                    new GameContextOutputArgs()
                    {
                        Type = Models.Enums.GameContextActionType.Verify,
                        CurrentFile = 0,
                        MaxFile = Resources.Count,
                        Progress = 0.0,
                    }
                );
                List<Resource> resources = new();
                for (int i = 0; i < Resources.Count; i++)
                {
                    this.IsVerify = true;
                    var file = folder + this.Resources[i].Dest.Replace('/', '\\');
                    if (!File.Exists(file))
                    {
                        resources.Add(this.Resources[i]);
                        double exis = (((double)i + 1) / this.Resources.Count) * 100;
                        this.gameContextOutputDelegate?.Invoke(
                            this,
                            new GameContextOutputArgs()
                            {
                                Type = Models.Enums.GameContextActionType.Verify,
                                CurrentFile = i + 1,
                                MaxFile = Resources.Count,
                                Progress = Math.Round(exis, 2),
                            }
                        );
                        continue;
                    }
                    var md5String = GetFileMD5(file);
                    if (md5String != this.Resources[i].Md5)
                    {
                        resources.Add(this.Resources[i]);
                        continue;
                    }

                    double value = (((double)i + 1) / this.Resources.Count) * 100;
                    if (!verifyCts.IsCancellationRequested)
                    {
                        this.gameContextOutputDelegate?.Invoke(
                            this,
                            new GameContextOutputArgs()
                            {
                                Type = Models.Enums.GameContextActionType.Verify,
                                CurrentFile = i + 1,
                                MaxFile = Resources.Count,
                                Progress = Math.Round(value, 2),
                            }
                        );
                    }
                    else
                    {
                        this.IsLaunch = true;
                        this.IsVerify = false;
                        this.gameContextOutputDelegate?.Invoke(
                            this,
                            new GameContextOutputArgs()
                            {
                                Type = Models.Enums.GameContextActionType.None,
                            }
                        );
                        return;
                    }
                }
                if (resources.Count > 0)
                {
                    this.IsVerify = false;
                    _downloadCTS = new();
                    //进入下载
                    await DownloadGameFiles(GameDownloadActionSource.Verify, folder, resources);
                    return;
                }
                await this.GameLocalConfig.SaveConfigAsync(
                    GameLocalSettingName.GameLauncherBassFolder,
                    folder
                );
                await this.GameLocalConfig.SaveConfigAsync(
                    GameLocalSettingName.GameLauncherBassProgram,
                    folder + "\\Wuthering Waves.exe"
                );
                await this.GameLocalConfig.SaveConfigAsync(
                    GameLocalSettingName.LocalGameResourceVersion,
                    launcherIndex.Default.ResourceChunk.LastVersion
                );
                await this.GameLocalConfig.SaveConfigAsync(
                    GameLocalSettingName.LocalGameVersion,
                    launcherIndex.Default.Version
                );
                this.IsLaunch = true;
                this.IsVerify = false;

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

    public void StartDownloadGame(
        string folder,
        WavesIndex waves,
        GameResource resource,
        bool isNew
    )
    {
        this.IsPause = false;
        if (IsDownload)
            return;
        Task.Run(async () =>
        {
            try
            {
                this.HttpClientService.BuildClient();
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
                this.Cdn = launcherIndex
                    .Default.CdnList.Where(p => p.P > 0)
                    .OrderByDescending(p => p.P)
                    .LastOrDefault();
                this.DownloadBaseUrl = Cdn.Url + WavesIndex.Default.ResourcesBasePath;
                var version = launcherIndex.Default.ResourceChunk.LastVersion;
                var resourceUrl = Cdn.Url + launcherIndex.Default.Resources;
                this.Resources = (await this.GetGameResourceAsync(resourceUrl)).Resource;
                var maxSize = Resources.Sum(x => x.Size);
                this.gameContextOutputDelegate?.Invoke(
                    this,
                    new GameContextOutputArgs()
                    {
                        Type = Models.Enums.GameContextActionType.Download,
                        CurrentFile = 0,
                        MaxFile = Resources.Count,
                        Progress = 0.0,
                        CurrentSize = 0,
                        Speed = 0,
                        SpeedString = "0MB/s",
                        MaxSize = maxSize,
                    }
                );
                this._downloadCTS = new();
                await DownloadGameFiles(
                    GameDownloadActionSource.Download,
                    folder,
                    Resources.ToList(),
                    this.WavesIndex.Default.ResourceChunk.LastVersion,
                    this.WavesIndex.Default.Version
                );
            }
            catch (IOException ex)
            {
                this.IsDownload = false;
                this.IsVerify = false;
                this.gameContextOutputDelegate?.Invoke(
                    this,
                    new() { Type = GameContextActionType.Error, ErrorMessage = "网络断开！" }
                );
                this.Lock = new RateLimiter(SpeedValue * 1024 * 1024);
                return;
            }
            catch (HttpRequestException httpEx)
            {
                this.IsDownload = false;
                this.IsVerify = false;
                this.gameContextOutputDelegate?.Invoke(
                    this,
                    new() { Type = GameContextActionType.Error, ErrorMessage = "网络断开！" }
                );
                this.Lock = new RateLimiter(SpeedValue * 1024 * 1024);
                return;
            }
            catch (Exception ex)
            {
                this.IsDownload = false;
                this.IsVerify = false;
                this.gameContextOutputDelegate?.Invoke(
                    this,
                    new() { Type = GameContextActionType.Error, ErrorMessage = ex.Message }
                );
                this.Lock = new RateLimiter(SpeedValue * 1024 * 1024);
                return;
            }
        });
    }

    private async Task DownloadGameFiles(
        GameDownloadActionSource actionSource,
        string folder,
        List<Resource> resources,
        string resourceVersion = "",
        string version = ""
    )
    {
        try
        {
            if (_downloadCTS.Token.IsCancellationRequested)
                return;
            if (this.IsDownload)
                return;
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
            long maxSize = list.Sum(x => x.Size);
            var totalBytesRead = 0L;
            long nowFileSize = 0L;
            UpdateDownloadProgress(
                GameContextActionType.Download,
                0,
                resources.Count,
                0,
                maxSize,
                0
            );
            this.IsDownload = true;
            for (global::System.Int32 i = 0; i < list.Length; i++)
            {
                if (_downloadCTS.Token.IsCancellationRequested)
                    return;
                string cachefile = "";
                if (actionSource == GameDownloadActionSource.Download)
                {
                    cachefile = folder + "\\DownloadCache" + list[i].Dest.Replace('/', '\\');
                }
                else
                {
                    cachefile = folder + list[i].Dest.Replace('/', '\\');
                }
                Directory.CreateDirectory(System.IO.Path.GetDirectoryName(cachefile)!);
                var url = DownloadBaseUrl + list[i].Dest;
                nowFileSize = 0;
                var request = new HttpRequestMessage()
                {
                    RequestUri = new(url),
                    Method = HttpMethod.Get,
                };
                FileStream fs = null;
                if (File.Exists(cachefile))
                {
                    var size = ReadFileSize(cachefile);
                    var md5 = GetFileMD5(cachefile);
                    if (md5 == list[i].Md5)
                    {
                        var progresssValue = Math.Round(
                            Convert.ToDouble((double)totalBytesRead / maxSize) * 100,
                            2
                        );
                        UpdateDownloadProgress(
                            GameContextActionType.Download,
                            i,
                            list.Length,
                            totalBytesRead,
                            maxSize,
                            0
                        );
                        Debug.WriteLine(progresssValue);
                        totalBytesRead += size;
                        continue;
                    }
                    if (actionSource == GameDownloadActionSource.Download)
                    {
                        if (size > list[i].Size)
                        {
                            File.Delete(cachefile);
                            fs = new FileStream(cachefile, FileMode.Create, FileAccess.Write);
                        }
                        else
                        {
                            request.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(
                                size,
                                null
                            );
                            totalBytesRead += size;
                            fs = new FileStream(cachefile, FileMode.Append, FileAccess.Write);
                        }
                    }
                    else if (
                        actionSource == GameDownloadActionSource.Verify
                        || actionSource == GameDownloadActionSource.Update
                    )
                    {
                        File.Delete(cachefile);
                        fs = new FileStream(cachefile, FileMode.Create, FileAccess.Write);
                    }
                }
                else
                {
                    fs = new FileStream(cachefile, FileMode.Create, FileAccess.Write);
                }
                using (
                    var response = await HttpClientService.GameDownloadClient.SendAsync(
                        request,
                        HttpCompletionOption.ResponseHeadersRead,
                        _downloadCTS.Token
                    )
                )
                {
                    if (_downloadCTS.Token.IsCancellationRequested)
                        return;
                    response.EnsureSuccessStatusCode();
                    using (var responseStream = await response.Content.ReadAsStreamAsync())
                    using (fs)
                    {
                        int bytesRead;
                        var startTime = DateTime.UtcNow;
                        var lastReportTime = DateTime.UtcNow;
                        var bytesSinceLastReport = 0L;
                        while (
                            (
                                bytesRead = await MaybeLimitAndReadAsync(
                                    responseStream,
                                    fs,
                                    IsLimitSpeed,
                                    Lock
                                )
                            ) > 0
                        )
                        {
                            if (_downloadCTS.Token.IsCancellationRequested)
                            {
                                await fs.FlushAsync();
                                fs.Close();
                                await fs.DisposeAsync();
                                break;
                            }
                            totalBytesRead += bytesRead;
                            nowFileSize += bytesRead;
                            bytesSinceLastReport += bytesRead;
                            var currentTime = DateTime.UtcNow;
                            var reportInterval = TimeSpan.FromSeconds(1);
                            if (currentTime - lastReportTime >= reportInterval)
                            {
                                var speed =
                                    bytesSinceLastReport
                                    / (currentTime - lastReportTime).TotalSeconds;
                                var progresssValue = Math.Round(
                                    Convert.ToDouble((double)totalBytesRead / maxSize) * 100,
                                    2
                                );
                                lastReportTime = currentTime;
                                bytesSinceLastReport = 0;
                                UpdateDownloadProgress(
                                    GameContextActionType.Download,
                                    i,
                                    list.Length,
                                    totalBytesRead,
                                    maxSize,
                                    speed,
                                    $"{speed / 1024 / 1024:F2} MB"
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
                        UpdateDownloadProgress(
                            GameContextActionType.Download,
                            i,
                            list.Length,
                            totalBytesRead,
                            maxSize,
                            speed2,
                            $"{speed2 / 1024 / 1024:F2} MB"
                        );
                        await fs.FlushAsync();
                    }
                }
            }
            this.gameContextOutputDelegate?.Invoke(
                this,
                new GameContextOutputArgs()
                {
                    Type = Models.Enums.GameContextActionType.Download,
                    CurrentSize = totalBytesRead,
                    MaxSize = maxSize,
                    Speed = 0.0,
                    SpeedString = "0MB",
                    CurrentFile = list.Length,
                    MaxFile = list.Length,
                    Progress = Math.Round(
                        Convert.ToDouble((double)totalBytesRead / maxSize) * 100,
                        2
                    ),
                }
            );
            this.gameContextOutputDelegate?.Invoke(
                this,
                new GameContextOutputArgs() { Type = Models.Enums.GameContextActionType.None }
            );
            this.IsDownload = false;
            if (actionSource == GameDownloadActionSource.Download)
            {
                await ClearGameCacheAsync(folder, this.Resources, resourceVersion, version);
                this.IsClear = true;
                return;
            }
            var exeFile = await GameLocalConfig.GetConfigAsync(
                GameLocalSettingName.GameLauncherBassProgram
            );
            Task.Run(async () => StartVerifyGame(folder));
        }
        catch (System.IO.IOException ioEx)
        {
            this.gameContextOutputDelegate?.Invoke(
                this,
                new() { Type = GameContextActionType.Error, ErrorMessage = "网络断开！" }
            );
            this.IsDownload = false;
            this.Lock = new RateLimiter(SpeedValue * 1024 * 1024);
        }
        catch (HttpRequestException httpex)
        {
            this.gameContextOutputDelegate?.Invoke(
                this,
                new() { Type = GameContextActionType.Error, ErrorMessage = httpex.Message }
            );
            this.IsDownload = false;
            this.Lock = new RateLimiter(SpeedValue * 1024 * 1024);
        }
        catch (Exception ex)
        {
            this.gameContextOutputDelegate?.Invoke(
                this,
                new() { Type = GameContextActionType.Error, ErrorMessage = ex.Message }
            );
            this.IsDownload = false;
            this.Lock = new RateLimiter(SpeedValue * 1024 * 1024);
        }
    }

    private readonly Queue<double> _speedHistory = new();
    private const int SpeedHistoryWindowSize = 5; // 滑动窗口大小（最近 5 秒）

    private double CalculateAverageSpeed(double currentSpeedInMBps)
    {
        // 转换为字节速度
        var currentSpeedInBytesPerSecond = currentSpeedInMBps * 1024 * 1024;

        // 添加最新速度到历史队列
        _speedHistory.Enqueue(currentSpeedInBytesPerSecond);

        // 如果队列超过设定的窗口大小，则移除最早的速度
        if (_speedHistory.Count > SpeedHistoryWindowSize)
        {
            _speedHistory.Dequeue();
        }

        // 返回平均速度（仍以字节单位）
        return _speedHistory.Average();
    }

    private void UpdateDownloadProgress(
        GameContextActionType type,
        int currentFile,
        int maxFile,
        long currentSize,
        long maxSize,
        double speed = 0.0,
        string speedString = "0MB/s"
    )
    {
        try
        {
            var progress = Math.Round((double)currentSize / maxSize * 100, 2);
            var averageSpeed = CalculateAverageSpeed(speed);
            // 计算剩余时间
            string remainingTimeString = "N/A";
            var speedValue = speed;
            if (averageSpeed > 0)
            {
                var remainingBytes = maxSize - currentSize;
                var remainingTime = TimeSpan.FromSeconds(remainingBytes / speedValue);
                remainingTimeString = $"{remainingTime:hh\\:mm\\:ss}";
            }

            this.gameContextOutputDelegate?.Invoke(
                this,
                new GameContextOutputArgs()
                {
                    Type = type,
                    CurrentFile = currentFile,
                    MaxFile = maxFile,
                    CurrentSize = currentSize,
                    MaxSize = maxSize,
                    Speed = speed,
                    SpeedString = speedString,
                    Progress = progress,
                    RemainingTime = remainingTimeString,
                }
            );
        }
        catch (Exception ex) { }
    }

    private long ReadFileSize(string cachefile)
    {
        var file = new FileInfo(cachefile);
        return file.Length;
    }

    private async Task ClearGameCacheAsync(
        string folder,
        List<Resource> resources,
        string resourceVersion = "",
        string version = ""
    )
    {
        try
        {
            this._clearCTS = new CancellationTokenSource();
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
                if (_clearCTS.IsCancellationRequested)
                    return;
                var cachefile = folder + "\\DownloadCache" + resources[i].Dest.Replace('/', '\\');
                var file = folder + resources[i].Dest.Replace('/', '\\');
                if (!File.Exists(cachefile))
                {
                    clearResource.Add(resources[i]);
                    continue;
                }
                var hash = this.GetFileMD5(cachefile);
                if (hash != null && hash == resources[i].Md5)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(file)!);
                    File.Move(cachefile, file);
                    var progress = Math.Round(
                        Convert.ToDouble((double)i / resources.Count) * 100,
                        2
                    );
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
            await GameLocalConfig.SaveConfigAsync(
                GameLocalSettingName.GameLauncherBassProgram,
                $"{folder}\\Wuthering Waves.exe"
            );
            if (!string.IsNullOrWhiteSpace(resourceVersion))
                await GameLocalConfig.SaveConfigAsync(
                    GameLocalSettingName.LocalGameResourceVersion,
                    resourceVersion
                );
            if (!string.IsNullOrWhiteSpace(version))
                await GameLocalConfig.SaveConfigAsync(
                    GameLocalSettingName.LocalGameVersion,
                    resourceVersion
                );
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
        catch (Exception)
        {
            return;
        }
    }

    async Task<int> MaybeLimitAndReadAsync(
        Stream responseStream,
        FileStream fileStream,
        bool isLimitSpeed,
        RateLimiter rateLimiter
    )
    {
        var buffer = new byte[819200]; // 80KB 缓冲区
        int bytesRead = await responseStream.ReadAsync(buffer, 0, buffer.Length);
        if (isLimitSpeed)
        {
            await rateLimiter.ConsumeAsync(bytesRead);
        }
        await fileStream.WriteAsync(buffer, 0, bytesRead);
        return bytesRead;
    }

    public string GetFileMD5(string file, CancellationToken token = default)
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
                    if (token != default && token.IsCancellationRequested)
                        return "";
                    hex.AppendFormat("{0:x2}", b);
                }
                md5String = hex.ToString();
            }
        }
        return md5String;
    }

    public async Task CancelDownloadAsync()
    {
        if (this._downloadCTS == null)
            return;
        else
            await _downloadCTS.CancelAsync();
        this.IsVerify = false;
        this.IsDownload = false;
        this.gameContextOutputDelegate?.Invoke(this, new() { Type = GameContextActionType.None });
    }

    public async Task ClearGameResourceAsync()
    {
        if (this._downloadCTS != null)
            await _downloadCTS.CancelAsync();
        if (this._clearCTS != null)
            await _clearCTS.CancelAsync();
        var gameFolder = await GameLocalConfig.GetConfigAsync(
            GameLocalSettingName.GameLauncherBassFolder
        );
        Directory.Delete(gameFolder, true);
        await GameLocalConfig.SaveConfigAsync(GameLocalSettingName.GameLauncherBassProgram, "");
        await GameLocalConfig.SaveConfigAsync(GameLocalSettingName.GameLauncherBassFolder, "");
        await GameLocalConfig.SaveConfigAsync(GameLocalSettingName.LocalGameResourceVersion, "");
        this.IsDownload = false;
        this.IsVerify = false;
        this.IsPause = false;
        this.IsLaunch = false;
        this.gameContextOutputDelegate?.Invoke(
            this,
            new GameContextOutputArgs() { Type = GameContextActionType.None }
        );
    }

    public async Task<bool> CheckUpdateAsync(CancellationToken token = default)
    {
        var folder = await this.GameLocalConfig.GetConfigAsync(
            GameLocalSettingName.GameLauncherBassFolder
        );
        var resourceVersion = await this.GameLocalConfig.GetConfigAsync(
            GameLocalSettingName.LocalGameResourceVersion
        );
        var localVersion = await this.GameLocalConfig.GetConfigAsync(
            GameLocalSettingName.LocalGameVersion
        );
        this.WavesIndex = await this.GetGameIndexAsync(token);

        if (
            resourceVersion == WavesIndex.Default.ResourceChunk.LastVersion
            && localVersion == WavesIndex.Default.Version
        )
        {
            return true;
        }
        StartVerifyGame(folder);
        return false;
    }

    public async Task StopGameVerify()
    {
        if (verifyCts != null)
        {
            await this.verifyCts.CancelAsync();
        }
        this.IsVerify = false;
        this.gameContextOutputDelegate?.Invoke(
            this,
            new GameContextOutputArgs() { Type = GameContextActionType.None }
        );
    }

    public async Task<GameContextConfig> ReadContextConfigAsync(CancellationToken token = default)
    {
        GameContextConfig config = new();
        var speed = await this.GameLocalConfig.GetConfigAsync(GameLocalSettingName.LimitSpeed);
        var dx11 = await this.GameLocalConfig.GetConfigAsync(GameLocalSettingName.IsDx11);
        if (int.TryParse(speed, out var rate))
        {
            config.LimitSpeed = rate;
        }
        else
            config.LimitSpeed = 0;
        if (string.IsNullOrWhiteSpace(dx11))
            config.IsDx11 = false;
        if (bool.TryParse(dx11, out var isDx11))
        {
            config.IsDx11 = isDx11;
        }
        else
            config.IsDx11 = false;
        return config;
    }

    public async Task<bool> SetLimitSpeedAsync(int value, CancellationToken token = default)
    {
        if (
            await this.GameLocalConfig.SaveConfigAsync(
                GameLocalSettingName.LimitSpeed,
                value.ToString()
            )
        )
        {
            this.SpeedValue = value;
            this.IsLimitSpeed = true;
            return true;
        }
        return false;
    }

    public async Task<bool> SetDx11LauncheAsync(bool value, CancellationToken token = default)
    {
        if (
            await this.GameLocalConfig.SaveConfigAsync(
                GameLocalSettingName.IsDx11,
                value.ToString()
            )
        )
        {
            this.IsDx11Launche = value;
        }
        return false;
    }
}
