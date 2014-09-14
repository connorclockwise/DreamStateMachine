using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DreamStateMachine.Behaviors;

namespace DreamStateMachine.Actions
{
    class Alert:Behavior
    {
        Actor owner;
        Actor target;
        World world;

        public Alert(ActionList ownerList, Actor owner, Actor target)
        {
            this.ownerList = ownerList;
            this.owner = owner;
            world = owner.world;
            this.target = target;
            elapsed = 0;
            duration = -1;
        }

        override public void onStart()
        {
            
        }

        override public void onEnd()
        {
            //ownerList.remove(this);
        }

        override public void update(float dt)
        {
            if (world.isInSight(owner, target.hitBox.Center))
            {
                ownerList.endAll();
                //Follow follow = new Follow(enemy.behaviorList, enemy, protagonist);
                //if (!enemy.behaviorList.has(follow))
                //    enemy.behaviorList.pushFront(follow);
                Aggravated aggravated = new Aggravated(ownerList, owner, target);
                if (!ownerList.has(aggravated))
                    ownerList.pushFront(aggravated);
                //Follow follow = new Follow(ownerList, owner, target, world);
                //if (!owner.behaviorList.has(follow))
                //    owner.behaviorList.pushFront(follow);
                //onEnd();
            }
        }
    }
}
