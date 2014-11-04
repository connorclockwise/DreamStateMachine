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

        public SpriteFont spriteFont;
        public String contents;
        public Vector2 pos;
        public Color color;

        public Label(SpriteFont spriteFont, String contents):base()
        {
            this.spriteFont = spriteFont;
            this.contents = contents;
            color = Color.White;
            pos = new Vector2(0, 0);
        }

        public override void draw(SpriteBatch spriteBatch, Rectangle drawSpace, Texture2D debugTex, bool debugging = false)
        {
            pos.X = dimensions.X;
            pos.Y = dimensions.Y;
            spriteBatch.DrawString(spriteFont, contents, pos, color);
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
            pos.X = (float)x - spriteFont.MeasureString(contents).X / 2;
            pos.Y = (float)y - spriteFont.MeasureString(contents).Y / 2; ;
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
