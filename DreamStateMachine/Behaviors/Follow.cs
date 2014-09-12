using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DreamStateMachine.Behaviors;

namespace DreamStateMachine.Actions
{
    class Follow:Behavior
    {
        ActionList ownerList;
        ActorManager actorManager;
        Actor owner;
        Actor target;
        World world;
        Point ownerTilePos;
        Point pathTilePos;

        public Follow(ActionList ownerList, Actor actor, Actor toFollow, World w, ActorManager aC)
        {
            actorManager = aC;
            this.ownerList = ownerList;
            this.owner = owner;
            target = toFollow;
            world = w;
            nextPathPoint = new Point(0,0);
            elapsed = 0;
            duration = -1;
            isBlocking = true;
        }

        override public void onStart()
        {
            path = world.findPath(owner.hitBox.Center, target.hitBox.Center);
            if (path.Count > 0)
                nextPathPoint = path[0];
            else
                onEnd();
        }

        override public void onEnd()
        {
            ownerList.remove(this);
        }

        override public void update(float dt)
        {
            ownerTilePos = new Point(owner.hitBox.Center.X / world.tileSize, owner.hitBox.Center.Y / world.tileSize);
            pathTilePos = new Point(nextPathPoint.X / world.tileSize, nextPathPoint.Y / world.tileSize);
            if (ownerTilePos.X == pathTilePos.X && ownerTilePos.Y == pathTilePos.Y)
            {
                if (path.Count > 0)
                {
                    nextPathPoint = path.ElementAt(0);
                    path.RemoveAt(0);
                }
                else
                {
                    onEnd();
                }
            }
            else 
            {
                Vector2 movement = new Vector2(nextPathPoint.X - owner.hitBox.Center.X, nextPathPoint.Y - owner.hitBox.Center.Y);
                movement.Normalize();
                movement *= 5;
                //owner.movementIntent /= 3f;
                owner.velocity.X = movement.X;
                owner.velocity.Y = movement.Y;
                owner.isWalking = true;
                owner.setGaze(nextPathPoint);
            }


        }
    }
}
