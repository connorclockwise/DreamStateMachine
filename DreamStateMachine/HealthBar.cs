using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace DreamStateMachine
{
    class HealthBar: IDrawable
    {
        Actor actor;
        Rectangle barContainer;
        Rectangle bar;
        Texture2D barTexture;
        //int width = 30;
        int barHeight = 10;
        int displaceX = 0;
        int displaceY = -30;

        public HealthBar(Actor actor, Texture2D barTex)
        {
            this.actor = actor;
            bar = new Rectangle(0,0, 30, 10);
            barContainer = new Rectangle(0, 0, 30, 10);
            barTexture = barTex;
        }

        public void draw(SpriteBatch spriteBatch, Rectangle drawSpace, bool debugging = false)
        {
            float healthPercentage = ((float)actor.health / actor.maxHealth);
            barContainer.X = actor.hitBox.Center.X - (barContainer.Width / 2) - drawSpace.X + displaceX;
            barContainer.Y = actor.hitBox.Center.Y - (barContainer.Height / 2) - drawSpace.Y + displaceY;
            bar.X = barContainer.X;
            bar.Y = barContainer.Y;
            bar.Width = (int)(barContainer.Width * healthPercentage);
            //bar.Height = barContainer.Height;
            spriteBatch.Draw(barTexture, barContainer, Color.Black);   
            spriteBatch.Draw(barTexture, bar, Color.White);   

        }

        public bool isInDrawSpace(Rectangle drawSpace)
        {
            return bar.Intersects(drawSpace);
        }
    }
}
