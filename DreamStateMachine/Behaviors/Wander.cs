using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DreamStateMachine.Behaviors;

namespace DreamStateMachine.Actions
{
    class Wander:Behavior
    {
        ActionList ownerList;
        ActorManager actorManager;
        Actor owner;
        List<Point> searchPath;
        World world;
        Random random;
        Point ownerTilePos;
        Point pathTilePos;
       


        public Wander(ActionList ownerList, Actor owner)
        {
            this.ownerList = ownerList;;
            this.owner = owner;
            elapsed = 0;
            duration = -1;
            searchPath = new List<Point>();
            nextPathPoint = new Point(0, 0);
            isBlocking = true;
            world = owner.world;
            random = new Random();
        }

        override public void onStart()
        {
            Point randomPos;
            randomPos = new Point(owner.hitBox.Center.X + random.Next(-200, 200), owner.hitBox.Center.Y + random.Next(-200, 200));

            while (!world.isInBounds(randomPos))
            {
                randomPos = new Point(owner.hitBox.Center.X + random.Next(-120, 120), owner.hitBox.Center.Y + random.Next(-120, 120));
            }

            path = world.findPath(owner.hitBox.Center, randomPos);

            if (path.Count > 0)
                nextPathPoint = path[0];
            else
                onEnd();
        }

        override public void onEnd()
        {
            ownerList.remove(this);
            owner.isWalking = false;
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
                movement *= 2;
                owner.velocity.X = movement.X;
                owner.velocity.Y = movement.Y;
                owner.isWalking = true;
                owner.setGaze(nextPathPoint);
            }


        }
    }
}
