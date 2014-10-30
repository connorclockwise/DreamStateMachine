using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
//using MonoGame.Framework;
using DreamStateMachine.Actions;


namespace DreamStateMachine
{
    class Door:Prop
    {

        public Door(Texture2D tex, int width, int height, int texWidth, int texHeight):base(tex, width, height, texWidth, texHeight)
        {
            Actor.Use += new EventHandler<EventArgs>(Actor_Use);
        }

        public override Object Clone()
        {
            Door DoorCopy = new Door(texture, hitBox.Width, hitBox.Height, body.Width, body.Height);
            
            DoorCopy.className = className;
            DoorCopy.texture = texture;
            DoorCopy.color = color;

            return DoorCopy;
        }

        public void setAnimationFrame(int x, int y)
        {
            curAnimFrame.X = x;
            curAnimFrame.Y = y;
        }

        private void Actor_Use(object sender, EventArgs args)
        {
            Actor usingActor = (Actor)sender;
            if (usingActor.health > 0)
            {
                int reach = usingActor.reach;
                Vector2 sightVector = usingActor.sightVector;
                Point usePoint = new Point();
                usePoint.X = (int)(usingActor.hitBox.Center.X + (int)(sightVector.X * reach));
                usePoint.Y = (int)(usingActor.hitBox.Center.Y + (int)(sightVector.Y * reach));
                if (hitBox.Contains(usePoint))
                {
                    onRemove();
                }
            }
        }


    }
}
