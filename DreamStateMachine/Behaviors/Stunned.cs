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
        Actor owner;

        public Stunned(ActionList ownerList, Actor owner)
        {
            this.ownerList = ownerList;
            this.owner = owner;
            duration = 3/12f;
            isBlocking = true;
        }

        override public void onStart()
        {
            //owner.animationList.endAll();
        }

        override public void onEnd()
        {
            ownerList.remove(this);
        }

        override public void update(float dt)
        {
            elapsed += dt;
        }
    }
}
