using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WutheringWavesTool.Common.Adaptives
{
    public class BoolToSettingAdaptive : IAdaptive<bool, string>
    {
        public static BoolToSettingAdaptive Instance { get; } = new();

        public string GetBack(bool forward)
        {
            return forward.ToString();
        }

        public bool GetForward(string value)
        {
            return Convert.ToBoolean(value);
        }
    }
}
