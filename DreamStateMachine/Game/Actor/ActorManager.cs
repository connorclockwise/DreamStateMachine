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
        Actor player;

        public ActorManager()
        {
            actorPrototypes = new Dictionary<string,Actor>();
            animationPrototypes = new Dictionary<string, AnimationInfo>();
            random = new Random();

            Actor.Death += new EventHandler<EventArgs>(Actor_Death);
            WorldManager.worldChange += new EventHandler<EventArgs>(World_Change);
        }

        void Actor_Death(object sender, EventArgs e)
        {
            if (((Actor)sender).className == "slime")
            {
                float x = ((Actor)sender).hitBox.Center.X / ((Actor)sender).world.tileSize;
                float y = ((Actor)sender).hitBox.Center.Y / ((Actor)sender).world.tileSize;
                List<SpawnFlag> spawns = new List<SpawnFlag>();
                SpawnFlag enemySpawn = new SpawnFlag("slimejr", new Point((int)x, (int)y), 2);
                //SpawnFlag enemySpawn2 = new SpawnFlag("slimejr", new Point((int)x, (int)y), 2);
                spawns.Add(enemySpawn);
                //spawns.Add(enemySpawn2);
                spawnActors(spawns);
            }
        }

        public void initActorConfig(ContentManager content, String actorConfigFile)
        {
            XDocument actorDoc = XDocument.Load(actorConfigFile);
            List<XElement> actors = actorDoc.Element("Actors").Elements("Actor").ToList();
            List<XElement> actorAnimations; 
            List<XElement> actorItems;
            String actorClass;
            String animationName;
            String animationType;
            Texture2D actorTexture;
            float actorDamageFactor;
            int actorMaxSpeed;
            int actorWidth;
            int actorHeight;
            int actorHealth;
            int actorSight;
            int actorReach;
            int texWidth;
            int texHeight;

            String itemClassName;
            float itemWeight;

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
                actorDamageFactor = float.Parse(actor.Attribute("damageFactor").Value);
                actorSight = int.Parse(actor.Attribute("sight").Value);
                actorReach = int.Parse(actor.Attribute("reach").Value);
                actorPrototypes[actorClass] = new Actor(actorTexture, actorWidth, actorHeight, texWidth, texHeight);
                actorPrototypes[actorClass].className = actorClass;
                actorPrototypes[actorClass].maxHealth = actorHealth;
                actorPrototypes[actorClass].health = actorHealth;
                actorPrototypes[actorClass].damageFactor = actorDamageFactor;
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

                actorItems = actor.Elements("Item").ToList();

                foreach (XElement item in actorItems) {
                    itemClassName = item.Attribute("className").Value;
                    itemWeight = float.Parse(item.Attribute("weight").Value);

                    actorPrototypes[actorClass].lootTable.Add(itemClassName, itemWeight);
                }

            }
        }

        public void initAnimationConfig(ContentManager content, String animationConfigFile)
        {
            List<String> meleeAttackTypes = new List<String> { "single_box_attack", "multi_box_attack" };
            List<String> projectileAttackTypes = new List<String> { "projectile_attack" };

            XDocument animationDoc = XDocument.Load(animationConfigFile);
            List<XElement> animations = animationDoc.Element("Animations").Elements("Animation").ToList();
            List<XElement> frames;
            List<XElement> attackBoxes;
            String animationName;
            String animationAttackType;
            bool animationHoldingWeapon;
            int animationFrames;
            int animationFPS;
            int texColumnIndex;
            int texRowIndex;

            int frameIndex;
            String weaponStance;
            float weaponRotation;
            int attackOffsetX;
            int attackOffsetY;
            int attackWidth;
            int attackHeight;
            Rectangle attackRect;
            List<Rectangle> attackRects;
            FrameInfo[] frameInfos;
            FrameInfo frameInfo;
            int attackDamage;
            List<Rectangle> attackPoints;
            Point gripPoint;


            foreach (XElement animation in animations)
            {
                animationName = animation.Attribute("name").Value;
                animationAttackType = animation.Attribute("attackType").Value;
                animationHoldingWeapon = bool.Parse(animation.Attribute("holdingWeapon").Value);
                animationFrames = int.Parse(animation.Attribute("frames").Value);
                animationFPS = int.Parse(animation.Attribute("fps").Value);
                texColumnIndex = int.Parse(animation.Attribute("columnIndex").Value);
                texRowIndex = int.Parse(animation.Attribute("rowIndex").Value);
                
                frames = animation.Elements("Frame").ToList();
                frameInfos = new FrameInfo[animationFrames];
                if (meleeAttackTypes.Contains(animationAttackType) || animationHoldingWeapon)
                {
                    
                    foreach (XElement frame in frames)
                    {
                        frameIndex = int.Parse(frame.Attribute("frameIndex").Value);
                        frameInfo = new FrameInfo();
                        attackRects = new List<Rectangle>();
                        if(animationHoldingWeapon){
                            gripPoint = new Point();
                            gripPoint.X = int.Parse(frame.Attribute("gripX").Value);
                            gripPoint.Y = int.Parse(frame.Attribute("gripY").Value);
                            weaponStance = frame.Attribute("stance").Value;
                            weaponRotation = float.Parse(frame.Attribute("rotation").Value);
                            frameInfo.gripPoint = gripPoint;
                            frameInfo.stance = weaponStance;
                            frameInfo.rotation = weaponRotation;

                        }
                        if (meleeAttackTypes.Contains(animationAttackType))
                        {
                            attackPoints = new List<Rectangle>();
                            attackDamage = int.Parse(frame.Attribute("damage").Value);
                            frameInfo.attackDamage = attackDamage;
                            attackBoxes = frame.Elements("AttackBox").ToList();
                            foreach (XElement attackBox in attackBoxes)
                            {
                                attackOffsetX = int.Parse(attackBox.Attribute("viewOffsetX").Value);
                                attackOffsetY = int.Parse(attackBox.Attribute("viewOffsetY").Value);
                                attackWidth = int.Parse(attackBox.Attribute("attackWidth").Value);
                                attackHeight = int.Parse(attackBox.Attribute("attackHeight").Value);
                                attackRect = new Rectangle(attackOffsetX, attackOffsetY, attackWidth, attackHeight);
                                attackPoints.Add(attackRect);
                            }
                            frameInfo.attackPoints = attackPoints;
                        }
                        frameInfos[frameIndex] = frameInfo;
                    }

                }

                animationPrototypes[animationName] = new AnimationInfo
                {
                    frames = frameInfos,
                    name = animationName,
                    frameCount = animationFrames,
                    type = animationAttackType,
                    fps = animationFPS,
                    texColumn = texColumnIndex,
                    texRow = texRowIndex
                };
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
                    Point spawnTile = spawn.tilePosition;
                    Vector2 newSightVector = new Vector2((float)random.NextDouble() * 2 - 1, (float)random.NextDouble() * 2 - 1);
                    Actor actorToCopy;
                    if (spawn.className.Equals("player") && player != null)
                    {
                        actorToCopy = (Actor)player.Clone();
                    }
                    else
                    {
                        actorToCopy = (Actor)actorPrototypes[spawn.className].Clone();
                        if (spawn.hasKey)
                            actorToCopy.hasKey = true;
                        
                    }
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
            player = worldManager.playerTransfer;
            if (worldManager.curWorld != null)
            {
                List<SpawnFlag> spawns = worldManager.curWorld.getSpawns();
                spawnActors(spawns);
            }
            player = null;
        }
    }
}
