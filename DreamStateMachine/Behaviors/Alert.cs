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
        ActionList ownerList;
        ActorController actorController;
        Enemy owner;
        Actor target;
        World world;

        public Alert(ActionList ol, Enemy o, Actor t, World w, ActorController aC)
        {
            actorController = aC;
            ownerList = ol;
            owner = o;
            world = w;
            target = t;
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
                owner.behaviorList.endAll();
                //Follow follow = new Follow(enemy.behaviorList, enemy, protagonist);
                //if (!enemy.behaviorList.has(follow))
                //    enemy.behaviorList.pushFront(follow);
                Aggravated aggravated = new Aggravated(owner.behaviorList, owner, target, world, actorController);
                if (!owner.behaviorList.has(aggravated))
                    owner.behaviorList.pushFront(aggravated);
                //Follow follow = new Follow(ownerList, owner, target, world);
                //if (!owner.behaviorList.has(follow))
                //    owner.behaviorList.pushFront(follow);
                //onEnd();
            }
        }
    }
}
