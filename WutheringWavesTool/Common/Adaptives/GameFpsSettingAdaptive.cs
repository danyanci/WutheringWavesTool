using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WutheringWavesTool.Common.Adaptives
{
    internal class GameFpsSettingAdaptive : IAdaptive<bool, string>
    {
        public static GameFpsSettingAdaptive Default => new GameFpsSettingAdaptive();

        public string GetBack(bool forward)
        {
            return forward ? "True" : "False";
        }

        public bool GetForward(string value)
        {
            return value == "True" ? true : false;
        }
    }
}
