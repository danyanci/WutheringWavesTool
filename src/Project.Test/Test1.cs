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
            GameContextFactory.GameBassPath =
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Waves";
            var mainGame = Register.ServiceProvider.GetRequiredKeyedService<IGameContext>(
                nameof(MainGameContext)
            );
            await mainGame.InitAsync();
            var result = await mainGame.GetLocalDLSSAsync();
            var dlssG = await mainGame.GetLocalDLSSGenerateAsync();
        }
    }
}
