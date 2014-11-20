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
    class WeaponItem:Prop, IUseable
    {
        public WeaponItem(Texture2D tex, String className, int width, int height, int offsetX, int offsetY, int texWidth, int texHeight):base(tex, width, height, texWidth, texHeight)
        {
            body.X = offsetX;
            body.Y = offsetY;
            body.Width = texWidth;
            body.Height = texHeight;
            this.className = className;
            //Actor.Use += new EventHandler<EventArgs>(Actor_Use);
            setAnimationFrame(1, 0);
        }

        public override Object Clone()
        {
            WeaponItem WeaponCopy = new WeaponItem(texture, className, hitBox.Width, hitBox.Height, body.X, body.Y, body.Width, body.Height);
            WeaponCopy.color = color;

            return WeaponCopy;
        }

        public void Use(Actor actor)
        {
            if (actor.activeWeapon != null)
            {
                actor.onDrop(actor.activeWeapon.name);
            }
            actor.onPickup(className);
            SoundManager.Instance.playSound("pickup");
            actor.pickUpCoolDown = .25f;
            onRemove();
        }

        public bool CanUse(Actor actor)
        {
            if (actor.health > 0 && actor.pickUpCoolDown <= 0)
            {

                int reach = actor.reach;
                Vector2 sightVector = actor.sightVector;
                Rectangle usePoint = new Rectangle(0, 0, 20, 20);
                usePoint.X = (int)(actor.hitBox.Center.X + (int)(sightVector.X * reach)) - usePoint.Width / 2;
                usePoint.Y = (int)(actor.hitBox.Center.Y + (int)(sightVector.Y * reach)) - usePoint.Height / 2;

                if (body.Intersects(actor.body))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
