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
        public List<Rectangle> attackRects;

        public DamageInfo(Actor a, int d, List<Rectangle> aR)
        {
            attacker = a;
            damage = d;
            attackRects = aR;
        }

    }
}
