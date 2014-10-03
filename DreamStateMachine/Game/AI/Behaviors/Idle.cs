using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DreamStateMachine.Behaviors;

namespace DreamStateMachine.Actions
{
    class Idle:Behavior
    {
        Actor owner;
        float nextWander;
        Random random;
        World world;

        public Idle(ActionList ownerList, Actor owner)
        {
            this.ownerList = ownerList;
            this.owner = owner;
            elapsed = 0;
            duration = -1;
            world = owner.world;
            random = new Random();
        }

        override public void onStart()
        {
            //owner.setBodyAnimationFrame(0, 0);
            nextWander = random.Next(6, 15);
        }

        override public void onEnd()
        {

        }

        override public void update(float dt)
        {
            elapsed += dt;
            if (elapsed > nextWander)
            {
                elapsed = 0;
                nextWander = random.Next(3, 10);
                Wander wander = new Wander(ownerList, owner);
                if (!ownerList.has(wander))
                {
                    ownerList.pushFront(wander);
                }
            }
        }
    }
}
