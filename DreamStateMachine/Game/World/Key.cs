using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using DreamStateMachine.Actions;


namespace DreamStateMachine
{
    class Key:Prop
    {
        public Key(Texture2D tex, int width, int height, int texWidth, int texHeight):base(tex, width, height, texWidth, texHeight)
        {
            Actor.Use += new EventHandler<EventArgs>(Actor_Use);
        }

        public override Object Clone()
        {
            Key KeyCopy = new Key(texture, hitBox.Width, hitBox.Height, body.Width, body.Height);

            KeyCopy.className = className;
            KeyCopy.texture = texture;
            KeyCopy.color = color;

            return KeyCopy;
        }

        private void Actor_Use(object sender, EventArgs args)
        {
            Actor usingActor = (Actor)sender;
            if (usingActor.health > 0)
            {

                int reach = usingActor.reach;
                Vector2 sightVector = usingActor.sightVector;
                Rectangle usePoint = new Rectangle(0,0,20,20);
                usePoint.X = (int)(usingActor.hitBox.Center.X + (int)(sightVector.X * reach)) - usePoint.Width / 2;
                usePoint.Y = (int)(usingActor.hitBox.Center.Y + (int)(sightVector.Y * reach)) - usePoint.Height / 2;

                if (body.Intersects(usePoint))
                {
                    onRemove();
                    SoundManager.Instance.playSound("pickup");
                    usingActor.hasKey = true;
                }

            }
        }
    }
}
