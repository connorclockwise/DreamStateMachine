using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DreamStateMachine.Behaviors;

namespace DreamStateMachine.Actions
{
    class Pursue:Behavior
    {
        ActionList ownerList;
        Actor owner;
        Actor target;
        Rectangle pathTileRectangle;

        public Pursue(ActionList ownerList, Actor owner, Actor toFollow)
        {
            this.ownerList = ownerList;
            this.owner = owner;
            target = toFollow;
            nextPathPoint = new Point(0,0);
            elapsed = 0;
            duration = -1;
            isBlocking = true;
            pathTileRectangle = new Rectangle(0, 0, owner.world.tileSize / 4, owner.world.tileSize / 4);
        }

        override public void onStart()
        {
            path = owner.world.findPath(owner.hitBox.Center, target.hitBox.Center);
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
            //traceInfo = owner.world.traceWorld(owner.hitBox.Center, target.hitBox.Center);
            

            pathTileRectangle.X = nextPathPoint.X - pathTileRectangle.Width / 2;
            pathTileRectangle.Y = nextPathPoint.Y - pathTileRectangle.Height / 2;
            if (owner.hitBox.Intersects(pathTileRectangle))
            {
                if (owner.world.isInSight(owner, target.hitBox.Center))
                {
                    onEnd();
                }
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
                movement *= owner.maxSpeed;
                owner.velocity.X = movement.X;
                owner.velocity.Y = movement.Y;
                owner.isWalking = true;
                owner.setGaze(nextPathPoint);
            }


        }
    }
}
