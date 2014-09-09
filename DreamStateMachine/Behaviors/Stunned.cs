using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DreamStateMachine.Behaviors;

namespace DreamStateMachine.Actions
{
    class Stunned:Behavior
    {
        ActionList ownerList;
        Enemy owner;

        public Stunned(ActionList ol, Enemy o)
        {
            ownerList = ol;
            owner = o;
            duration = 0.4f;
            isBlocking = true;
        }

        override public void onStart()
        {
            //duration = (5f/12f);
            //owner.lockMovement();
            owner.animationList.endAll();
        }

        override public void onEnd()
        {
            //if (owner.isWalking)
            //    elapsed = 0;
            //else
            //    owner.setFeetAnimationFrame(0, 0);

            //owner.setBodyAnimationFrame(0, 0);
            ownerList.remove(this);
            //owner.unlockMovement();
        }

        override public void update(float dt)
        {
            elapsed += dt;
        }
    }
}
