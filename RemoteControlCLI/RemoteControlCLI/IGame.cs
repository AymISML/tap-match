using System;
using System.Collections.Generic;
using System.Text;

namespace RemoteControlCLI
{
    public interface IGame
    {
        void Run(NetActor netActor);
    }
}
