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
        ActorController actorController;
        Actor owner;
        double dotProduct;
        

        public Recoil(ActionList ol, Actor o)
        {
            ownerList = ol;
            owner = o;
            duration = .83F;
            isBlocking = true;
        }

        override public void onStart()
        {
            elapsed = 0;
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
