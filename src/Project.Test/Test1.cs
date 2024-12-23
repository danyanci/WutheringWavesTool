using Microsoft.Extensions.DependencyInjection;
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

        void GetAge(out string age)
        {
            age = "Out的参数！不过只有一个捏";
        }

        Tuple<bool,string> GetAge()
        {
            IProgress<int> progress = new Progress<int>();
            progress.
        }

        public Task ProgressMethod(IProgress<int> progress)
        {
            return Task.Run(async () =>
            {
                progress = new Progress<int>();
                (progress as Progress<int>).ProgressChanged += Test1_ProgressChanged;
                void Test1_ProgressChanged(object? sender, int e)
                {
                    Console.WriteLine($"当前值{e}");
                }
                for (int i = 0; i < 10000; i++)
                {
                    //耗时操作
                    await Task.Delay(100);
                    progress.Report(i);
                }
            });
        }


        [TestMethod]
        public async Task TestMethod1()
        {
            int a = 15;
            GetAge(out var str);
            Console.WriteLine($"{str}");
            Console.WriteLine(GetAge().Item1);
        }
    }
}
