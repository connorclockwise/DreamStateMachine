using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using DreamStateMachine.Game.GUI;

namespace DreamStateMachine
{
    class Button: UIComponent
    {
        private Panel basePanel;
        private Color originalPanelColor;

        public Button(Texture2D tex):base()
        {
            dimensions = new Rectangle(0, 0, 0, 0);
            basePanel = new Panel(tex, Color.Gray);
            originalPanelColor = new Color(basePanel.color, 255);
            basePanel.dimensions = dimensions;
            children = new List<UIComponent>();
            canHaveFocus = true;
        }

        public override void draw(SpriteBatch spriteBatch, Rectangle drawSpace, Texture2D debugTex, bool debugging = false)
        {
            spriteBatch.Draw(basePanel.tex, dimensions, basePanel.color);
            foreach (UIComponent uiChild in children)
            {
                if (uiChild.dimensions.Intersects(dimensions))
                {
                    uiChild.draw(spriteBatch, drawSpace, debugTex, debugging);
                }
            }

        }

        public override void giveFocus()
        {

            basePanel.color.R = 255;
            basePanel.color.G = 255;
            basePanel.color.B = 255;
            hasFocus = true;
        }

        public override void takeFocus()
        {
            //basePanel.color = new Color(originalPanelColor, 255);
            basePanel.color.R = originalPanelColor.R;
            basePanel.color.G = originalPanelColor.G;
            basePanel.color.B = originalPanelColor.B;
            hasFocus = false;
        }

        public override bool isInDrawSpace(Rectangle drawSpace)
        {
            return dimensions.Intersects(drawSpace);
        }

        public override void setPos(int x, int y)
        {
            dimensions.X = x;
            dimensions.Y = y;
        }

        public override void setPos(Point pos)
        {
            dimensions.X = pos.X;
            dimensions.Y = pos.Y;
        }

        public override UIComponent findFocusedChild()
        {
            throw new NotImplementedException();
        }
    }
}
