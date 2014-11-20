using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using DreamStateMachine.Actions;
using DreamStateMachine2.game.World;


namespace DreamStateMachine
{
    class Key : Prop, IUseable
    {
        public Key(Texture2D tex, int width, int height, int texWidth, int texHeight):base(tex, width, height, texWidth, texHeight)
        {
            //Actor.Use += new EventHandler<EventArgs>(Actor_Use);
        }

        public override Object Clone()
        {
            Key KeyCopy = new Key(texture, hitBox.Width, hitBox.Height, body.Width, body.Height);

            KeyCopy.className = className;
            KeyCopy.texture = texture;
            KeyCopy.color = color;

            return KeyCopy;
        }

        public void Use(Actor actor)
        {
            SoundManager.Instance.playSound("pickup");
            actor.hasKey = true;
            onRemove();
        }

        public bool CanUse(Actor actor)
        {
            if (actor.health > 0)
            {
                int reach = actor.reach;
                Vector2 sightVector = actor.sightVector;
                Point usePoint = new Point();
                usePoint.X = (int)(actor.hitBox.Center.X + (int)(sightVector.X * reach));
                usePoint.Y = (int)(actor.hitBox.Center.Y + (int)(sightVector.Y * reach));

                if (body.Intersects(actor.body))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
