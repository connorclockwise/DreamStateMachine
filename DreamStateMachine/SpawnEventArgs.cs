using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DreamStateMachine
{
    class SpawnEventArgs : EventArgs
    {
        public int spawnType;

        public SpawnEventArgs(int spawnType):base()
        {
            this.spawnType = spawnType;
        }
    }
}
