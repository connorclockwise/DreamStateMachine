using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using DreamStateMachine.Game.GUI;

namespace DreamStateMachine
{
    class Panel: UIComponent
    {
        public Color color;
        public Texture2D tex;

        public Panel(Texture2D tex, Color color):base()
        {
            this.color = color;
            this.tex = tex;
            dimensions = new Rectangle(0, 0, 0, 0);
            children = new List<UIComponent>();
            onClick = click;
        }

        public void addChild(UIComponent child){
            child.dimensions.X += dimensions.X;
            child.dimensions.Y += dimensions.Y;
            child.parent = this;
            children.Add(child);
        }

        private void click()
        {
        }

        public void center()
        {
            if (parent != null)
            {
                dimensions.X = parent.dimensions.Width / 2 - dimensions.Width / 2;
                dimensions.X = parent.dimensions.Height / 2 - dimensions.Height / 2;
            }
        }

        public void centerHorizontally()
        {
            if(parent != null){
                dimensions.X = parent.dimensions.Width / 2 - dimensions.Width / 2;
            }
        }

        public void centerVertically()
        {
            if (parent != null)
            {
                dimensions.X = parent.dimensions.Height / 2 - dimensions.Height / 2;
            }
        }

        public override void draw(SpriteBatch spriteBatch, Rectangle drawSpace, Texture2D debugTex, bool debugging = false)
        {
            spriteBatch.Draw(tex, dimensions, color);
            foreach (UIComponent uiChild in children)
            {
                if (uiChild.dimensions.Intersects(dimensions))
                {
                    uiChild.draw(spriteBatch, drawSpace, debugTex, debugging);
                }
            }
        }

        public override UIComponent findFocusedChild()
        {
            foreach(UIComponent child in children){
                if (child.hasFocus)
                    return child;
            }
            return null;
        }

        public override void giveFocus()
        {
        }

        public override void takeFocus()
        {
        }

        public override void setPos(Point pos)
        {
            Point offset = new Point(pos.X - dimensions.X, pos.Y - dimensions.Y);
            dimensions.X = pos.X;
            dimensions.Y = pos.Y;
            
            foreach(UIComponent child in children){
                child.setPos(child.dimensions.X + offset.X, child.dimensions.Y + offset.Y);
            }
        }

        public override void setPos(int x, int y)
        {
            Point offset = new Point(x - dimensions.X, y - dimensions.Y);
            dimensions.X = x;
            dimensions.Y = y;

            foreach (UIComponent child in children)
            {
                child.setPos(child.dimensions.X + offset.X, child.dimensions.Y + offset.Y);
            }
        }

        public override bool isInDrawSpace(Rectangle drawSpace)
        {
            return dimensions.Intersects(drawSpace);
        }
    }
}
