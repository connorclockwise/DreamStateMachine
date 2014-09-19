using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DreamStateMachine
{
    class SpawnEventArgs : EventArgs
    {
        public Point spawnTile;
        public int spawnType;

        public SpawnEventArgs(Point spawnTile, int spawnType):base()
        {
            this.spawnTile = spawnTile;
            this.spawnType = spawnType;
        }
    }
}
