using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DreamStateMachine
{
    class TraceInfo
    {
        public Point hitPos;
        public bool hitWorld;
        public bool hitActor;

        public TraceInfo(bool hw, bool ha, Point hp)
        {
            hitWorld = hw;
            hitActor = ha;
            hitPos = hp;
        }

        public TraceInfo(bool hw, bool ha)
        {
            hitWorld = hw;
            hitActor = ha;
        }

    }
}
