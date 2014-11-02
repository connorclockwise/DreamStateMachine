﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DreamStateMachine.Actions;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Xml.Linq;

namespace DreamStateMachine.Behaviors
{
    class PhysicsController
    {
        List<Actor> actors;
        List<Prop> props;
        World world;
        Random random;
        Rectangle predictedMove;
        int actorX;
        int actorY;
        int actorW;
        int actorH;

        public PhysicsController()
        {

            actors = new List<Actor>();
            props = new List<Prop>();
            random = new Random();

            Actor.Spawn += new EventHandler<SpawnEventArgs>(Actor_Spawn);
            Prop.Spawn += new EventHandler<SpawnEventArgs>(Prop_Spawn);
            Prop.Remove += new EventHandler<EventArgs>(Prop_Remove);
            Actor.Death += new EventHandler<EventArgs>(Actor_Death);
            WorldManager.worldChange +=new EventHandler<EventArgs>(World_Change);
        }

        private void Actor_Spawn(object sender, SpawnEventArgs e)
        {
            Actor spawnedActor = (Actor)sender;
            Point spawnPoint = new Point(e.spawnTile.X * world.tileSize, e.spawnTile.Y * world.tileSize);
            spawnedActor.setPos(spawnPoint);
            actors.Add(spawnedActor);
            
        }

        private void Prop_Spawn(object sender, SpawnEventArgs e)
        {
            Prop spawnedProp = (Prop)sender;
            Point spawnPoint = new Point(e.spawnTile.X * world.tileSize, e.spawnTile.Y * world.tileSize);
            spawnedProp.setPos(spawnPoint);
            props.Add(spawnedProp);
        }

        private void Prop_Remove(object sender, EventArgs e)
        {
            Prop toRemoveProp = (Prop)sender;
            props.Remove(toRemoveProp);
        }

        private void Actor_Death(object sender, EventArgs e)
        {
            Actor deadActor = (Actor)sender;
            actors.Remove(deadActor);
        }

        public void update(float dt){
            foreach (Actor actor in actors)
            {
                this.handlePropCollision(actor);
                this.handleMapCollision(actor);
                this.handleMovement(actor);
            }
        }

        public bool collidesWithProp(Prop prop,Rectangle rect)
        {
            if(prop.hitBox.Intersects(rect)){
                return true;
            }
            return false;
        }

        public bool collidesWithProp(Prop prop, Point point)
        {
            if (prop.hitBox.Contains(point))
            {
                return true;
            }
            return false;
        }

        public void handlePropCollision(Actor actor)
        {
            Rectangle intersectedRect = new Rectangle();
            Point newPos = new Point();
            foreach (Prop prop in props)
            {
                if (collidesWithProp(prop, actor.hitBox))
                {
                    Rectangle.Intersect(ref actor.hitBox,ref prop.hitBox,out intersectedRect);
                    if (actor.velocity.X < 0)
                    {
                        newPos.X = actor.hitBox.X + intersectedRect.Width + 1;   
                    }
                    else if (actor.velocity.X > 0)
                    {
                        newPos.X = actor.hitBox.X - intersectedRect.Width - 1;
                    }
                    else
                    {
                        newPos.X = actor.hitBox.X;
                    }

                    if (actor.velocity.Y < 0)
                    {
                        newPos.Y = actor.hitBox.Y + intersectedRect.Height + 1;
                    }
                    else if (actor.velocity.Y > 0)
                    {
                        newPos.Y = actor.hitBox.Y - intersectedRect.Height - 1;
                    }else{
                        newPos.Y = actor.hitBox.Y;
                    }

                    if(!collidesWithProp(prop, new Point(newPos.X, actor.hitBox.Y))){
                        actor.setPos(new Point(newPos.X, actor.hitBox.Y));
                    }
                    else if (!collidesWithProp(prop, new Point(actor.hitBox.X, newPos.Y)))
                    {
                        actor.setPos(new Point(actor.hitBox.X, newPos.Y));
                    }
                    else{
                        actor.setPos(newPos);
                    }
                    
                }
                
            }
        }

        public void handleMapCollision(Actor actor)
        {
            actorX = actor.hitBox.X;
            actorY = actor.hitBox.Y;
            actorW = actor.hitBox.Width;
            actorH = actor.hitBox.Height;

            predictedMove.X = actorX + (int)Math.Round(actor.velocity.X);
            predictedMove.Y = actorY + (int)Math.Round(actor.velocity.Y);


            predictedMove.Width = actorW;
            predictedMove.Height = actorH;

            Point remainingMovement = new Point(0, 0);
            if (world.isInBounds(predictedMove))
            {
                actor.setPos(predictedMove.X, predictedMove.Y);
            }
            else
            {
                if (actor.velocity.X < 0)
                {
                    if (world.isInBounds(predictedMove.X, actorY) && world.isInBounds(predictedMove.X, actorY + actorH - 1))
                        remainingMovement.X = (int)actor.velocity.X;
                    else if (actorX % world.tileSize != 0)
                    {
                        remainingMovement.X = -(actorX % world.tileSize);
                    }
                }
                else if (actor.velocity.X > 0)
                {
                    if (world.isInBounds(predictedMove.X + actorW, actorY) && world.isInBounds(predictedMove.X + actorW, actorY + actorH - 1))
                        remainingMovement.X = (int)actor.velocity.X;
                    else if ((actorX + actorW) % world.tileSize != 0)
                        remainingMovement.X = world.tileSize - ((actorX + actorW) % world.tileSize);
                }

                if (actor.velocity.Y < 0)
                {
                    if (world.isInBounds(actorX, predictedMove.Y) && world.isInBounds(actorX + actorW - 1, predictedMove.Y))
                        remainingMovement.Y = (int)actor.velocity.Y;
                    else if (actorY % world.tileSize != 0)
                        remainingMovement.Y = -(actorY % world.tileSize);
                }
                else if (actor.velocity.Y > 0)
                {
                    if (world.isInBounds(actorX, predictedMove.Y + actorH) && world.isInBounds(actorX + actorW - 1, predictedMove.Y + actorH - 1))
                        remainingMovement.Y = (int)actor.velocity.Y;
                    else if ((actorY + actorH) % world.tileSize != 0)
                        remainingMovement.Y = world.tileSize - ((actorY + actorH) % world.tileSize);
                }
                predictedMove.X = actor.hitBox.X + remainingMovement.X;
                predictedMove.Y = actor.hitBox.Y + remainingMovement.Y;
                predictedMove.Width--;
                predictedMove.Height--;

                if (world.isInBounds(predictedMove))
                    actor.setPos(actor.hitBox.X + remainingMovement.X, actor.hitBox.Y + remainingMovement.Y);

            }            
        }

        public void handleMovement(Actor actor)
        {
            if (actor.movementIntent.X == 0)
            {
                actor.acceleration.X = 0;
                actor.velocity.X += actor.friction.X;
            }
            else
                actor.velocity.X += actor.acceleration.X;

            if (actor.movementIntent.Y == 0)
            {
                actor.acceleration.Y = 0;
                actor.velocity.Y += actor.friction.Y;
            }
            else
                actor.velocity.Y += actor.acceleration.Y;

            if (actor.acceleration.X == 0)
            {
                if (actor.velocity.X > 0)
                {
                    if (actor.velocity.X < Math.Abs(actor.friction.X))
                    {
                        actor.velocity.X = 0;
                        actor.friction.X = 0;
                    }
                    else
                    {
                        actor.friction.X = -.75f;
                    }
                }
                else if (actor.velocity.X < 0)
                {
                    if (Math.Abs(actor.velocity.X) < actor.friction.X)
                    {
                        actor.velocity.X = 0;
                        actor.friction.X = 0;
                    }
                    else
                    {
                        actor.friction.X = .75f;
                    }

                }
                else if (actor.velocity.X == 0)
                {
                    actor.friction.X = 0;
                }
            }

            if (actor.acceleration.Y == 0)
            {
                if (actor.velocity.Y > 0)
                {
                    if (actor.velocity.Y < Math.Abs(actor.friction.Y))
                    {
                        actor.velocity.Y = 0;
                        actor.friction.Y = 0;
                    }
                    else
                    {
                        actor.friction.Y = -.75f;
                    }
                }
                else if (actor.velocity.Y < 0)
                {
                    if (Math.Abs(actor.velocity.Y) < actor.friction.Y)
                    {
                        actor.velocity.Y = 0;
                        actor.friction.Y = 0;
                    }
                    else
                    {
                        actor.friction.Y = .75f;
                    }

                }
                else if (actor.velocity.Y == 0)
                {
                    actor.friction.Y = 0;
                }
            }

            actor.acceleration.X = actor.movementIntent.X;
            actor.acceleration.Y = actor.movementIntent.Y;


            if (actor.velocity.X > 0 && actor.acceleration.X > 0)
            {
                actor.velocity.X = Math.Max(actor.velocity.X, actor.minSpeed);
            }
            if (actor.velocity.X < 0 && actor.acceleration.X < 0)
            {
                actor.velocity.X = Math.Min(actor.velocity.X, -actor.minSpeed);
            }

            if (actor.velocity.Y > 0 && actor.acceleration.Y > 0)
            {
                actor.velocity.Y = Math.Max(actor.velocity.Y, actor.minSpeed);
            }
            if (actor.velocity.Y < 0 && actor.acceleration.Y < 0)
            {
                actor.velocity.Y = Math.Min(actor.velocity.Y, -actor.minSpeed);
            }

            actor.velocity.X = MathHelper.Clamp(actor.velocity.X, -actor.maxSpeed, actor.maxSpeed);
            actor.velocity.Y = MathHelper.Clamp(actor.velocity.Y, -actor.maxSpeed, actor.maxSpeed);

            //if (actor.movementIntent.X > 0)
            //{
            //    actor.acceleration.X = .3f;
            //    actor.velocity.X = MathHelper.Clamp(actor.velocity.X, actor.minSpeed, actor.maxSpeed * movementFactor);
            //}
            //else if (actor.movementIntent.X < 0)
            //{
            //    //actor.velocity.X = Math.Max(-actor.maxSpeed, actor.velocity.X);
            //    actor.acceleration.X = -.3f;
            //    actor.velocity.X = MathHelper.Clamp(actor.velocity.X, -actor.maxSpeed * movementFactor, -actor.minSpeed);
            //}

            //if (actor.movementIntent.Y > 0)
            //{
            //    //actor.velocity.Y = Math.Min(actor.maxSpeed, actor.velocity.Y);
            //    actor.acceleration.Y = .3f;
            //    actor.velocity.Y = MathHelper.Clamp(actor.velocity.Y, actor.minSpeed, actor.maxSpeed * movementFactor);
            //}
            //else if (actor.movementIntent.Y < 0)
            //{
            //    //actor.velocity.Y = Math.Max(-actor.maxSpeed, actor.velocity.Y);
            //    actor.acceleration.Y = -.3f;
            //    actor.velocity.Y = MathHelper.Clamp(actor.velocity.Y, -actor.maxSpeed * movementFactor, -actor.minSpeed);
            //}
        }

        private void World_Change(Object sender, EventArgs eventArgs)
        {
            actors.Clear();
            
            WorldManager worldManager = (WorldManager)sender;
            if(worldManager.curWorld != null)
                world = worldManager.curWorld;
        }
    }
}
