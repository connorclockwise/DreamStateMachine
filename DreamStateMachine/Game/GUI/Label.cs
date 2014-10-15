using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using DreamStateMachine.Game.GUI;

namespace DreamStateMachine
{
    class Label: UIComponent
    {

        SpriteFont spriteFont;
        String contents;
        Vector2 pos;

        public Label(SpriteFont spriteFont, String contents):base()
        {
            this.spriteFont = spriteFont;
            this.contents = contents;
            pos = new Vector2(0, 0);
        }

        public override void draw(SpriteBatch spriteBatch, Rectangle drawSpace, Texture2D debugTex, bool debugging = false)
        {
            spriteBatch.DrawString(spriteFont, contents, pos, Color.White);
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
