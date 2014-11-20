using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
//using MonoGame.Framework;
using DreamStateMachine.Actions;
using DreamStateMachine2.game.World;


namespace DreamStateMachine
{
    class Door : Prop, IUseable
    {

        public Door(Texture2D tex, int width, int height, int texWidth, int texHeight):base(tex, width, height, texWidth, texHeight)
        {
            //Actor.Use += new EventHandler<EventArgs>(Actor_Use);
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

        public void Use(Actor actor)
        {
            actor.hasKey = false;
            onRemove();
        }

        public bool CanUse(Actor actor)
        {
            if (actor.health > 0 && actor.hasKey)
            {
                int reach = actor.reach;
                Vector2 sightVector = actor.sightVector;
                Point usePoint = new Point();
                usePoint.X = (int)(actor.hitBox.Center.X + (int)(sightVector.X * reach));
                usePoint.Y = (int)(actor.hitBox.Center.Y + (int)(sightVector.Y * reach));
                if (body.Contains(usePoint))
                {
                    return true;
                }
            }
            return false;
        }


    }
}
