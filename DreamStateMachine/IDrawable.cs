using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace DreamStateMachine
{
    interface IDrawable
    {
        void draw(SpriteBatch spriteBatch, Rectangle drawSpace, Texture2D debugTex, bool debuging = false);
        bool isInDrawSpace(Rectangle drawSpace);
    }
}
