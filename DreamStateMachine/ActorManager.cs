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
        Random random;

        public ActorManager()
        {
            actorPrototypes = new Dictionary<string,Actor>();
            random = new Random();

            WorldManager.worldChange += new EventHandler<EventArgs>(World_Change);
        }

        public void initActorConfig(ContentManager content, String actorConfigFile)
        {
            var doc = XDocument.Load(actorConfigFile);
            var actors = doc.Element("Actors").Elements("Actor");
            String actorClass;
            Texture2D actorTexture;
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
                actorWidth = int.Parse(actor.Attribute("width").Value);
                actorHeight = int.Parse(actor.Attribute("height").Value);
                texWidth = int.Parse(actor.Attribute("texWidth").Value);
                texHeight = int.Parse(actor.Attribute("texHeight").Value);
                actorHealth = int.Parse(actor.Attribute("health").Value);
                actorSight = int.Parse(actor.Attribute("sight").Value);
                actorReach = int.Parse(actor.Attribute("reach").Value);
                this.actorPrototypes[actorClass] = new Actor(actorTexture, actorWidth, actorHeight, texWidth, texHeight);
                this.actorPrototypes[actorClass].className = actorClass;
                this.actorPrototypes[actorClass].health = actorHealth;
                this.actorPrototypes[actorClass].sight = actorSight;
                this.actorPrototypes[actorClass].reach = actorReach;
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
