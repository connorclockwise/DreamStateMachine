using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DreamStateMachine
{
    class SpawnFlag
    {
        public String className;
        public Point tilePosition;
        public int spawnType;
        public bool hasKey;

        public SpawnFlag(String cN, Point point, int type){
            className = cN;
            tilePosition = point;
            spawnType = type;
        }
    }
}
