using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using DreamStateMachine.Game.GUI;

namespace DreamStateMachine
{
    class MovingLabel: Label
    {

        public Vector2 velocity;

        public MovingLabel(SpriteFont spriteFont, String contents):base(spriteFont, contents)
        {
            velocity = new Vector2(0, 0);
        }

        public override void update(float dt)
        {
            this.dimensions.X += (int)velocity.X;
            this.dimensions.Y += (int)velocity.Y;
        }
    }
}
