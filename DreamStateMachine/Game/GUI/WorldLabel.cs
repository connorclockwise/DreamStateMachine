using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using DreamStateMachine.Game.GUI;

namespace DreamStateMachine
{
    class WorldLabel: Label
    {

        Vector2 normalizedPos;

        public WorldLabel(SpriteFont spriteFont, String contents):base(spriteFont, contents)
        {
            dimensions.Width = (int)spriteFont.MeasureString(contents).X;
            dimensions.Height = (int)spriteFont.MeasureString(contents).Y;
            normalizedPos = new Vector2();
        }

        public override void draw(SpriteBatch spriteBatch, Rectangle drawSpace, Texture2D debugTex, bool debugging = false)
        {
            if (dimensions.Intersects(drawSpace))
            {
                normalizedPos.X = dimensions.X - drawSpace.X;
                normalizedPos.Y = dimensions.Y - drawSpace.Y;
                spriteBatch.DrawString(base.spriteFont, base.contents, normalizedPos, Color.White);
            }
            
        }

        public override void giveFocus()
        {
            throw new NotImplementedException();
        }

        public override void takeFocus()
        {
            throw new NotImplementedException();
        }

        public override bool isInDrawSpace(Rectangle drawSpace)
        {
            return dimensions.Intersects(drawSpace);
        }

        public override void setPos(int x, int y)
        {
            pos.X = (float)x;
            pos.Y = (float)y;
        }

        public override void setPos(Point point)
        {
            pos.X = ((float)point.X);
            pos.Y = ((float)point.Y);
        }

        public override UIComponent findFocusedChild()
        {
            throw new NotImplementedException();
        }
    }
}
