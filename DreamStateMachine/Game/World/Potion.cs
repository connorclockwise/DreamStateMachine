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
    class Potion:Prop
    {

        public int restore;

        public Potion(Texture2D tex, int width, int height, int texWidth, int texHeight, int restore):base(tex, width, height, texWidth, texHeight)
        {
            Actor.Use += new EventHandler<EventArgs>(Actor_Use);
            this.restore = restore;
        }

        public override Object Clone()
        {
            Potion PotionCopy = new Potion(texture, hitBox.Width, hitBox.Height, body.Width, body.Height, restore);
            
            PotionCopy.className = className;
            PotionCopy.texture = texture;
            PotionCopy.color = color;

            return PotionCopy;
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
                if (body.Contains(usePoint))
                {
                    if (usingActor.health + restore <= usingActor.maxHealth)
                        usingActor.health += restore;
                    SoundManager.Instance.playSound("pickup");
                    onRemove();
                }
            }
        }
    }
}
