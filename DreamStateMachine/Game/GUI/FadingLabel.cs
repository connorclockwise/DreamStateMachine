using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using DreamStateMachine.Game.GUI;

namespace DreamStateMachine
{
    class FadingLabel: WorldLabel
    {
        public float timeToFade;
        public float curTime;

        public FadingLabel(SpriteFont spriteFont, String contents):base(spriteFont, contents)
        {
            timeToFade = 2f;
            curTime = 0;
        }

        public void update(float dt)
        {
            curTime += dt;
            if (timeToFade > curTime)
            {
                this.color.A = (byte)(255* (1 - (curTime / timeToFade)));
            }
        }
    }
}
