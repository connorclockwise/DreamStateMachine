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
        ActionList ownerList;
        //ActorController actorController;
        Actor owner;
        Color originalColor;
        //double dotProduct;
        

        public Recoil(ActionList ol, Actor o)
        {
            ownerList = ol;
            owner = o;
            duration = 2/12F;
            originalColor = o.color;
            isBlocking = true;
            
        }

        override public void onStart()
        {
            elapsed = 0;
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