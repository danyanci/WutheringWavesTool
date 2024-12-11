using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Waves.Core.GameContext;
using Waves.Core.Models;

namespace Waves.Core.GameContext.Contexts;

public class GlobalGameContext : GameContextBase
{
    internal GlobalGameContext(GameApiContextConfig config)
        : base(config, nameof(GlobalGameContext)) { }
}
