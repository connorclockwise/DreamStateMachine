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
    class ActorManager
    {

        Dictionary<String, Actor> actorPrototypes;
        Dictionary<String, AnimationInfo> animationPrototypes;
        Random random;

        public ActorManager()
        {
            actorPrototypes = new Dictionary<string,Actor>();
            animationPrototypes = new Dictionary<string, AnimationInfo>();
            random = new Random();

            WorldManager.worldChange += new EventHandler<EventArgs>(World_Change);
        }

        public void initActorConfig(ContentManager content, String actorConfigFile)
        {
            XDocument actorDoc = XDocument.Load(actorConfigFile);
            List<XElement> actors = actorDoc.Element("Actors").Elements("Actor").ToList();
            List<XElement> actorAnimations; 
            String actorClass;
            String animationName;
            String animationType;
            Texture2D actorTexture;
            int actorMaxSpeed;
            int actorWidth;
            int actorHeight;
            int actorHealth;
            int actorSight;
            int actorReach;
            int texWidth;
            int texHeight;

            foreach (XElement actor in actors)
            {
                actorClass = actor.Attribute("className").Value;
                actorTexture = content.Load<Texture2D>(actor.Attribute("texture").Value);
                actorMaxSpeed = int.Parse(actor.Attribute("maxSpeed").Value);
                actorWidth = int.Parse(actor.Attribute("width").Value);
                actorHeight = int.Parse(actor.Attribute("height").Value);
                texWidth = int.Parse(actor.Attribute("texWidth").Value);
                texHeight = int.Parse(actor.Attribute("texHeight").Value);
                actorHealth = int.Parse(actor.Attribute("health").Value);
                actorSight = int.Parse(actor.Attribute("sight").Value);
                actorReach = int.Parse(actor.Attribute("reach").Value);
                actorPrototypes[actorClass] = new Actor(actorTexture, actorWidth, actorHeight, texWidth, texHeight);
                actorPrototypes[actorClass].className = actorClass;
                actorPrototypes[actorClass].maxHealth = actorHealth;
                actorPrototypes[actorClass].health = actorHealth;
                actorPrototypes[actorClass].maxSpeed = actorMaxSpeed;
                actorPrototypes[actorClass].sight = actorSight;
                actorPrototypes[actorClass].reach = actorReach;
                actorPrototypes[actorClass].animations = new Dictionary<string, AnimationInfo>();
                actorAnimations = actor.Elements("Animation").ToList();
                
                foreach (XElement actorAnimation in actorAnimations)
                {
                    if (this.animationPrototypes.ContainsKey(actorAnimation.Attribute("name").Value))
                    {
                        animationName = actorAnimation.Attribute("name").Value;
                        animationType = actorAnimation.Attribute("type").Value;
                        actorPrototypes[actorClass].animations[animationType] = animationPrototypes[animationName];
                    }   
                }
            }
        }

        public void initAnimationConfig(ContentManager content, String animationConfigFile)
        {
            List<String> attackTypes = new List<String> { "single_box_attack", "multi_box_attack" };
            List<String> projectileAttackTypes = new List<String> { "projectile_attack" };

            XDocument animationDoc = XDocument.Load(animationConfigFile);
            List<XElement> animations = animationDoc.Element("Animations").Elements("Animation").ToList();
            List<XElement> attackFrames;
            List<XElement> attackBoxes;
            String animationName;
            String animationAttackType;
            int animationFrames;
            int animationFPS;
            int texColumnIndex;
            int texRowIndex;

            int currentAttackFrame;
            int attackOffsetX;
            int attackOffsetY;
            int attackWidth;
            int attackHeight;
            Rectangle attackRect;
            List<Rectangle> attackRects;
            Dictionary<int, int> attackDamage;
            Dictionary<int, List<Rectangle>> attackPoints;


            foreach (XElement animation in animations)
            {
                animationName = animation.Attribute("name").Value;
                if (animation.Attribute("attackType") != null)
                    animationAttackType = animation.Attribute("attackType").Value;
                else
                    animationAttackType = "";
                animationFrames = int.Parse(animation.Attribute("frames").Value);
                animationFPS = int.Parse(animation.Attribute("fps").Value);
                texColumnIndex = int.Parse(animation.Attribute("columnIndex").Value);
                texRowIndex = int.Parse(animation.Attribute("rowIndex").Value);
                if (attackTypes.Contains(animationAttackType))
                {
                    attackDamage = new Dictionary<int, int>();
                    attackPoints = new Dictionary<int, List<Rectangle>>();
                    attackFrames = animation.Elements("AttackFrame").ToList();
                    foreach (XElement attackFrame in attackFrames)
                    {
                        attackRects = new List<Rectangle>();
                        currentAttackFrame = int.Parse(attackFrame.Attribute("frame").Value);
                        attackDamage[currentAttackFrame] = int.Parse(attackFrame.Attribute("damage").Value);
                        attackBoxes = attackFrame.Elements("AttackBox").ToList();
                        foreach (XElement attackBox in attackBoxes)
                        {
                            attackOffsetX = int.Parse(attackBox.Attribute("viewOffsetX").Value);
                            attackOffsetY = int.Parse(attackBox.Attribute("viewOffsetY").Value);
                            attackWidth = int.Parse(attackBox.Attribute("attackWidth").Value);
                            attackHeight = int.Parse(attackBox.Attribute("attackHeight").Value);
                            attackRect = new Rectangle(attackOffsetX, attackOffsetY, attackWidth, attackHeight);
                            attackRects.Add(attackRect);
                        }
                        attackPoints[currentAttackFrame] = attackRects;

                    }
                    animationPrototypes[animationName] = new AnimationInfo(animationName,
                                                            animationFrames,
                                                            animationFPS,
                                                            texColumnIndex,
                                                            texRowIndex,
                                                            attackDamage,
                                                            attackPoints);
                }
                else
                {
                    animationPrototypes[animationName] = new AnimationInfo(animationName,
                                                            animationFrames,
                                                            animationFPS,
                                                            texColumnIndex,
                                                            texRowIndex);
                }
            }
        }

        public void spawnActor(Actor actor, Point spawnTile, int spawnType)
        {
            actor.onSpawn(spawnTile, spawnType);
            //actor.world = worldManager.curWorld;
        }

        public void spawnActors(List<SpawnFlag> spawns)
        {
            foreach (SpawnFlag spawn in spawns)
            {
                if (actorPrototypes.ContainsKey(spawn.className))
                {
                        Actor actorToCopy = (Actor)actorPrototypes[spawn.className].Clone();
                        Point spawnTile = spawn.tilePosition;
                        Vector2 newSightVector = new Vector2((float)random.NextDouble() * 2 - 1, (float)random.NextDouble() * 2 - 1);
                        actorToCopy.setGaze(newSightVector);
                        spawnActor(actorToCopy, spawnTile, spawn.spawnType);
                }
            }
        }

        public void respawnActors(List<SpawnFlag> spawns)
        {
            foreach (SpawnFlag spawn in spawns)
            {
                if (actorPrototypes.ContainsKey(spawn.className))
                {
                    Actor actorToCopy = (Actor)actorPrototypes[spawn.className].Clone();
                    Point spawnTile = spawn.tilePosition;
                    Vector2 newSightVector = new Vector2((float)random.NextDouble() * 2 - 1, (float)random.NextDouble() * 2 - 1);
                    actorToCopy.setGaze(newSightVector);
                    spawnActor(actorToCopy, spawnTile, spawn.spawnType);
                }
            }
        }


        private void World_Change(Object sender, EventArgs eventArgs)
        {
            WorldManager worldManager = (WorldManager)sender;
            List<SpawnFlag> spawns = worldManager.curWorld.getSpawns();
            spawnActors(spawns);
        }
    }
}
