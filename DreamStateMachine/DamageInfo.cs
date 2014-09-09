using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DreamStateMachine
{
    class DamageInfo
    {
        public Actor attacker;
        public int damage;
        public Rectangle attackRect;

        public DamageInfo(Actor a, int d, Rectangle aR)
        {
            attacker = a;
            damage = d;
            attackRect = aR;
        }

    }
}
