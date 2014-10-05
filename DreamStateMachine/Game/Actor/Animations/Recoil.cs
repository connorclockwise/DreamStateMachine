using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DreamStateMachine.Behaviors;

namespace DreamStateMachine.Actions
{
    class Recoil:Action
    {
        //actorManager actorManager;
        Actor owner;
        Color originalColor;
        //double dotProduct;

        public Recoil(ActionList ownerList, Actor owner)
        {
            this.ownerList = ownerList;
            this.owner = owner;
            duration = 2/12F;
            originalColor = new Color(owner.color.ToVector3());
            isBlocking = true;
        }

        override public void onStart()
        {
            elapsed = 0;
            owner.color = Color.Crimson;
        }

        override public void onEnd()
        {
            owner.color = originalColor;
            ownerList.remove(this);
        }

        override public void update(float dt)
        {
            elapsed += dt;
            //owner.color.R = (byte)(originalColor.R - ((elapsed / duration) * (originalColor.R - owner.color.R)));
            //owner.color.G = (byte)(originalColor.G - ((elapsed / duration) * (originalColor.G - owner.color.G)));
            //owner.color.B = (byte)(originalColor.B - ((elapsed / duration) * (originalColor.B - owner.color.B)));
            //owner.color.A = (byte)(originalColor.A - ((elapsed / duration) * (originalColor.A - owner.color.A)));
        }
    }
}