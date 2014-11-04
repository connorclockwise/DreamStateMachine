using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using DreamStateMachine.Actions;


namespace DreamStateMachine
{
    class WeaponItem:Prop
    {
        public WeaponItem(Texture2D tex, String className, int width, int height, int offsetX, int offsetY, int texWidth, int texHeight):base(tex, width, height, texWidth, texHeight)
        {
            body.X = offsetX;
            body.Y = offsetY;
            body.Width = texWidth;
            body.Height = texHeight;
            this.className = className;
            Actor.Use += new EventHandler<EventArgs>(Actor_Use);
            setAnimationFrame(1, 0);
        }

        public override Object Clone()
        {
            WeaponItem WeaponCopy = new WeaponItem(texture, className, hitBox.Width, hitBox.Height, body.X, body.Y, body.Width, body.Height);
            WeaponCopy.color = color;

            return WeaponCopy;
        }

        private void Actor_Use(object sender, EventArgs args)
        {
            Actor usingActor = (Actor)sender;
            if (usingActor.health > 0 && usingActor.pickUpCoolDown <= 0)
            {

                int reach = usingActor.reach;
                Vector2 sightVector = usingActor.sightVector;
                Rectangle usePoint = new Rectangle(0,0,20,20);
                usePoint.X = (int)(usingActor.hitBox.Center.X + (int)(sightVector.X * reach)) - usePoint.Width / 2;
                usePoint.Y = (int)(usingActor.hitBox.Center.Y + (int)(sightVector.Y * reach)) - usePoint.Height / 2;

                if (body.Intersects(usePoint))
                {
                    if (usingActor.activeWeapon != null)
                    {
                        usingActor.onDrop(usingActor.activeWeapon.name);
                    }
                    onRemove();
                    usingActor.onPickup(className);
                    SoundManager.Instance.playSound("pickup");
                    usingActor.pickUpCoolDown = .25f;
                }

            }
        }
    }
}
