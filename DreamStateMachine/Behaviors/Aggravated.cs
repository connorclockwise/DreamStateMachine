using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DreamStateMachine.Behaviors;

namespace DreamStateMachine.Actions
{
    class Aggravated:Behavior
    {
        ActionList ownerList;
        ActorController actorController;
        Enemy owner;
        Actor target;
        World world;
        Point ownerTilePos;
        Point pathTilePos;

        public Aggravated(ActionList ol, Enemy o, Actor toFollow, World w, ActorController aC)
        {
            actorController = aC;
            ownerList = ol;
            owner = o;
            target = toFollow;
            nextPathPoint = new Point(0,0);
            elapsed = 0;
            duration = -1;
            isBlocking = true;
        }

        override public void onStart()
        {
        }

        override public void onEnd()
        {
            ownerList.remove(this);
        }

        override public void update(float dt)
        {
            //Console.WriteLine(path.Count);
            double distance = Math.Sqrt(Math.Pow(target.hitBox.Center.X - owner.hitBox.Center.X, 2) + Math.Pow(target.hitBox.Center.Y - owner.hitBox.Center.Y, 2));
            if (distance > 50)
            {
                Vector2 attackVector = new Vector2(owner.hitBox.Center.X - target.hitBox.Center.X, owner.hitBox.Center.Y - target.hitBox.Center.Y);
                attackVector.Normalize();
                attackVector *= 50;
                attackVector.X += target.hitBox.Center.X;
                attackVector.Y += target.hitBox.Center.Y;
                Point attackPos = new Point((int)attackVector.X, (int)attackVector.Y);
                Vector2 movement = new Vector2(attackPos.X - owner.hitBox.Center.X, attackPos.Y - owner.hitBox.Center.Y);
                movement.Normalize();
                movement *= 5;
                //owner.movementIntent /= 3f;
                owner.velocity.X = movement.X;
                owner.velocity.Y = movement.Y;
                owner.isWalking = true;
                owner.setGaze(attackPos);
            }
            else
            {
                owner.setGaze(target.hitBox.Center);
                Punch punch = new Punch(owner.animationList, owner, actorController);
                if (!owner.animationList.has(punch))
                {
                    owner.animationList.pushFront(punch);
                }
            }
        }
    }
}
