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
            originalColor = owner.color;
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
            //owner.color.R = (byte)(255 - ((elapsed / duration) * (255 - originalColor.R)));
            //owner.color.G = (byte)(255 - ((elapsed / duration) * (255 - originalColor.G)));
            //owner.color.B = (byte)(255 - ((elapsed / duration) * (255 - originalColor.B)));
            //owner.color.A = (byte)(255 - ((elapsed / duration) * (255 - originalColor.A)));
            
            
        }
    }
}