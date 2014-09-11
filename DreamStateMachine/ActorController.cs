using System;
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
    class ActorController
    {
        Dictionary<String, Actor> actorPrototypes;
        List<Actor> actors;
        List<ActionList> behaviorLists;
        List<ActionList> actionLists;
        Actor curActor;
        Actor victim;
        Actor protagonist;
        Enemy enemy;
        Random random;
        Rectangle predictedMove;
        WorldManager worldManager;
        int actorX;
        int actorY;
        int actorW;
        int actorH;

        public ActorController(WorldManager w, List<Actor> a, Random r)
        {
            worldManager = w;
            actors = a;
            random = r;
            actorPrototypes = new Dictionary<string,Actor>();
            behaviorLists = new List<ActionList>();
        }

        public bool handleActorAttack(DamageInfo damageInfo)
        {
            for (int i = 0; i < actors.Count; i++)
            {
                victim = actors.ElementAt(i);
                if (victim.Equals(damageInfo.attacker))
                    continue;
                else if (victim.hitBox.Intersects(damageInfo.attackRect))
                {
                    //victim.velocity += actor.velocity * 5;

                    //Vector2 impulse = new Vector2(actor.sightVector.X, actor.sightVector.Y);
                    //actor.movementIntent.X += impulse.X;
                    //actor.movementIntent.Y += impulse.Y;
                    victim.velocity += damageInfo.attacker.sightVector * 20;
                    victim.onHurt(damageInfo);
                    Recoil recoil = new Recoil(victim.animationList, victim);
                    if (!victim.animationList.has(recoil))
                        victim.animationList.pushFront(recoil);
                    if (victim.GetType() == typeof(Enemy))
                    {
                        enemy = (Enemy)victim;
                        
                        Aggravated aggravated = new Aggravated(enemy.behaviorList, enemy, damageInfo.attacker, worldManager.getCurWorld(), this );
                        if (!enemy.behaviorList.has(aggravated))
                            enemy.behaviorList.pushFront(aggravated);
                        Stunned stunned = new Stunned(enemy.behaviorList, enemy);
                        if (!enemy.behaviorList.has(stunned))
                            enemy.behaviorList.pushFront(stunned);
                        
                    }


                    if (victim.health <= 0)
                    {
                        actors.Remove(victim);
                    }
                    return true;
                }
            }

            return false;
            
        }

        public void handleActorUse(Actor actor, Point usePoint)
        {
            int[,] tileMap = worldManager.curWorld.getTileMap();
            if (tileMap[usePoint.Y, usePoint.X] == 15)
            {
                //worldManager.curWorld.useTileAtPoint(usePoint);
                //Console.Write(worldManager.curLevel);
                if (worldManager.getWorldChild(0) == null)
                {
                    World newWorld = worldManager.createNextWorld(0);
                    actors.Clear();
                    //cam.world = newWorld;
                    //actorController.world = newWorld;
                    //actorController.spawnActor(player, newWorld.getSpawnPos());
                    this.spawnActor(protagonist, worldManager.curWorld.getSpawnPos());
                    this.spawnActors(worldManager.curWorld.getSpawns());
                }
                //isLoadingWorld = false;
                //Console.Write(worldManager.curLevel);
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
            if (worldManager.curWorld.isInBounds(predictedMove))
            {
                actor.setPos(predictedMove.X, predictedMove.Y);
            }
            else
            {
                if (actor.velocity.X < 0)
                {
                    if (worldManager.curWorld.isInBounds(predictedMove.X, actorY) && worldManager.curWorld.isInBounds(predictedMove.X, actorY + actorH - 1))
                        remainingMovement.X = (int)actor.velocity.X;
                    else if (actorX % worldManager.curWorld.tileSize != 0)
                    {
                        remainingMovement.X = -(actorX % worldManager.curWorld.tileSize);
                    }
                }
                else if (actor.velocity.X > 0)
                {
                    if (worldManager.curWorld.isInBounds(predictedMove.X + actorW, actorY) && worldManager.curWorld.isInBounds(predictedMove.X + actorW, actorY + actorH - 1))
                        remainingMovement.X = (int)actor.velocity.X;
                    else if ((actorX + actorW) % worldManager.curWorld.tileSize != 0)
                        remainingMovement.X = worldManager.curWorld.tileSize - ((actorX + actorW) % worldManager.curWorld.tileSize);
                }

                if (actor.velocity.Y < 0)
                {
                    if (worldManager.curWorld.isInBounds(actorX, predictedMove.Y) && worldManager.curWorld.isInBounds(actorX + actorW - 1, predictedMove.Y))
                        remainingMovement.Y = (int)actor.velocity.Y;
                    else if (actorY % worldManager.curWorld.tileSize != 0)
                        remainingMovement.Y = -(actorY % worldManager.curWorld.tileSize);
                }
                else if (actor.velocity.Y > 0)
                {
                    if (worldManager.curWorld.isInBounds(actorX, predictedMove.Y + actorH) && worldManager.curWorld.isInBounds(actorX + actorW - 1, predictedMove.Y + actorH - 1))
                        remainingMovement.Y = (int)actor.velocity.Y;
                    else if ((actorY + actorH) % worldManager.curWorld.tileSize != 0)
                        remainingMovement.Y = worldManager.curWorld.tileSize - ((actorY + actorH) % worldManager.curWorld.tileSize);
                }
                predictedMove.X = actor.hitBox.X + remainingMovement.X;
                predictedMove.Y = actor.hitBox.Y + remainingMovement.Y;
                predictedMove.Width--;
                predictedMove.Height--;

                if (worldManager.curWorld.isInBounds(predictedMove))
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

        public void initActorConfig(ContentManager content, String actorConfigFile)
        {
            var doc = XDocument.Load(actorConfigFile);
            var actors = doc.Element("Actors").Elements("Actor");
            String actorClass;
            Texture2D actorTexture;
            int actorWidth;
            int actorHeight;
            int texWidth;
            int texHeight;
            foreach (XElement actor in actors)
            {
                actorClass = actor.Attribute("className").Value;
                actorTexture = content.Load<Texture2D>(actor.Attribute("texture").Value);
                actorWidth = int.Parse(actor.Attribute("width").Value);
                actorHeight = int.Parse(actor.Attribute("height").Value);
                texWidth = int.Parse(actor.Attribute("texWidth").Value);
                texHeight = int.Parse(actor.Attribute("texHeight").Value);
                this.actorPrototypes[actorClass] = new Actor(actorTexture, actorWidth, actorHeight, texWidth, texHeight);
                this.actorPrototypes[actorClass].className = actorClass;
            }
       }

        public void setProtagonist(Actor a)
        {
            protagonist = a;
        }

        public void spawnActor(Actor actor, Point point){
            
            actor.setPos(point);
            actor.onSpawn();
            actors.Add(actor);
            if (actor.isPlayer)
                protagonist = actor;
            else if (actor.GetType() == typeof(Enemy))
            {
                enemy = (Enemy)actor;
                enemy.color = Color.Crimson;
                enemy.setGaze(new Vector2(random.Next(-1,1), random.Next(-1,1)));
                Idle idle = new Idle(enemy.behaviorList, enemy, worldManager.curWorld, random, this);
                enemy.behaviorList.pushFront(idle);
                Alert alert = new Alert(enemy.behaviorList, enemy, protagonist, worldManager.curWorld, this);
                enemy.behaviorList.pushFront(alert);
            }
        }

        public void spawnActors(List<SpawnFlag> spawns)
        {
            foreach (SpawnFlag spawn in spawns)
            {
                if (actorPrototypes.ContainsKey(spawn.className))
                {
                    if(spawn.spawnType == 2)
                    {
                        Actor actorToCopy = (Actor)actorPrototypes[spawn.className];
                        Enemy newEnemy = new Enemy(actorToCopy.texture, actorToCopy.hitBox.Width, actorToCopy.hitBox.Height, actorToCopy.body.Width, actorToCopy.body.Height);
                        Point spawnPoint = new Point(spawn.tilePosition.X * worldManager.curWorld.tileSize, spawn.tilePosition.Y * worldManager.curWorld.tileSize);
                        spawnActor(newEnemy, spawnPoint);
                    }
                }

                
            }
        }

        public void update(float dt)
        {

            for (int i = 0; i < actors.Count; i++)
            {

                curActor = actors.ElementAt(i);

                curActor.update(dt);

                this.handleMapCollision(curActor);
                this.handleMovement(curActor);

                //if (curActor.isAttacking)
                //{
                //    this.handleAttack(curActor);
                //}
            }
        }

    }
}
