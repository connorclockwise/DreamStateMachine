using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using DreamStateMachine.Game.GUI;

namespace DreamStateMachine
{
    class HealthBar: UIComponent
    {
        Actor actor;
        Rectangle bar;
        Rectangle barDimensions;
        Texture2D barTexture;
        int barWidth = 50;
        int barHeight = 15;
        int displaceX = 0;
        int displaceY = -30;

        public HealthBar(Actor actor, Texture2D barTex):base()
        {
            this.actor = actor;
            bar = new Rectangle(0, 0, barWidth, barHeight);
            dimensions = new Rectangle(0, 0, barWidth, barHeight);
            barTexture = barTex;
        }

        public override void draw(SpriteBatch spriteBatch, Rectangle drawSpace, Texture2D debugTex, bool debugging = false)
        {
            float healthPercentage = ((float)actor.health / actor.maxHealth);
            dimensions.X = actor.hitBox.Center.X - (dimensions.Width / 2) - drawSpace.X + displaceX;
            dimensions.Y = actor.hitBox.Center.Y - (dimensions.Height / 2) - drawSpace.Y + displaceY;
            //
            bar.Width = (int)(dimensions.Width * healthPercentage);
            bar.X = 0;
            bar.Y = 0;
            barDimensions.X = dimensions.X;
            barDimensions.Y = dimensions.Y;
            barDimensions.Width = bar.Width;
            barDimensions.Height = bar.Height;
            
            //bar.Height = dimensions.Height;
            spriteBatch.Draw(barTexture, dimensions, Color.Black); 
            //spriteBatch.Draw(barTexture, bar, Color.White);  
            spriteBatch.Draw(barTexture, barDimensions, bar, Color.White);
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
            return bar.Intersects(drawSpace);
        }

        public override void setPos(Point pos)
        {
        }

        public override void setPos(int x, int y)
        {
        }

        public override UIComponent findFocusedChild()
        {
            throw new NotImplementedException();
        }
    }
}
