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
    class Potion : Prop, IUseable
    {

        public int restore;

        public Potion(Texture2D tex, int width, int height, int texWidth, int texHeight, int restore):base(tex, width, height, texWidth, texHeight)
        {
            //Actor.Use += new EventHandler<EventArgs>(Actor_Use);
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

        public void Use(Actor actor)
        {
            if (actor.health > 0)
            {
                int reach = actor.reach;
                Vector2 sightVector = actor.sightVector;
                Point usePoint = new Point();
                usePoint.X = (int)(actor.hitBox.Center.X + (int)(sightVector.X * reach));
                usePoint.Y = (int)(actor.hitBox.Center.Y + (int)(sightVector.Y * reach));
                if (body.Contains(usePoint))
                {
                    if (actor.health + restore <= actor.maxHealth)
                        actor.health += restore;
                    SoundManager.Instance.playSound("pickup");
                    onRemove();
                }
            }
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
