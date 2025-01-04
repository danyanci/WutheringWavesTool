using System;
using System.Diagnostics;
using System.Security.Cryptography;
using Microsoft.CodeCoverage.Core.Reports.Coverage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Waves.Api.Models;
using Waves.Core.GameContext;
using Waves.Core.GameContext.Contexts;
using Waves.Core.Models;

namespace Project.Test
{
    [TestClass]
    public sealed class Test1
    {
        public Test1()
        {
            Register.Init();
        }

        [TestMethod]
        public async Task MyTestMethod()
        {
            var mainGame = Register.ServiceProvider.GetRequiredKeyedService<IGameContext>(
                nameof(MainGameContext)
            );
            var index = await mainGame.GetGameIndexAsync();
            var cdn = index
                .Default.CdnList.Where(p => p.P > 0)
                .OrderByDescending(p => p.P)
                .LastOrDefault();
            var resourceUrl = cdn.Url + index.Default.Resources;
            var resource = await mainGame.GetGameResourceAsync(resourceUrl);
            var totalSize = resource.Resource.Sum(x => x.Size);
            IProgress<double> progress = new Progress<double>(value =>
            {
                //Debug.WriteLine($"Progress: {value:P2}");
            });
            long processedBytes = 0;
            byte[] buffer = new byte[2048 * 2048];
            int bytesRead;
            List<Tuple<string, string>> md5s = new();
            Parallel.ForEach(
                resource.Resource,
                res =>
                {
                    var index = resource.Resource.IndexOf(res);
                    Debug.WriteLine(index);
                    var file = "D:\\Wuthering Waves\\Games" + res.Dest.Replace('/', '\\');
                    using (var md5 = MD5.Create())
                    using (var stream = File.OpenRead(file))
                    {
                        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            md5.TransformBlock(buffer, 0, bytesRead, null, 0);
                            processedBytes += bytesRead;

                            // 更新进度
                            double currentProgress = (double)processedBytes / totalSize;
                            progress.Report(currentProgress);
                        }
                        md5.TransformFinalBlock(buffer, 0, 0);
                        byte[] hashValue = md5.Hash;
                        string hashString = BitConverter
                            .ToString(hashValue)
                            .Replace("-", "")
                            .ToLowerInvariant();
                        md5s.Add(new Tuple<string, string>(hashString, file));
                    }
                }
            );
        }
    }
}
