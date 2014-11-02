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
    class PropManager
    {
        Dictionary<String, Prop> PropPrototypes;
        Dictionary<String, Door> DoorPrototypes;
        Dictionary<String, Potion> PotionPrototypes;
        Random random;

        public PropManager()
        {
            PropPrototypes = new Dictionary<string,Prop>();
            DoorPrototypes = new Dictionary<string,Door>();
            PotionPrototypes = new Dictionary<string, Potion>();
            random = new Random();

            WorldManager.worldChange += new EventHandler<EventArgs>(World_Change);
        }

        public void initPropConfig(ContentManager content, String PropConfigFile)
        {
            var doc = XDocument.Load(PropConfigFile);
            var Props = doc.Element("Props").Elements("Prop");
            var Doors = doc.Element("Props").Elements("Door");
            var Potions = doc.Element("Props").Elements("Potion");

            String PropClass;
            Texture2D PropTexture;
            int PropWidth;
            int PropHeight;

            String DoorClass;
            Texture2D DoorTexture;
            int DoorWidth;
            int DoorHeight;

            String PotionClass;
            Texture2D PotionTexture;
            int PotionWidth;
            int PotionHeight;
            int PotionRestore;

            int texWidth;
            int texHeight;
            foreach (XElement Prop in Props)
            {
                PropClass = Prop.Attribute("className").Value;
                PropTexture = content.Load<Texture2D>(Prop.Attribute("texture").Value);
                PropWidth = int.Parse(Prop.Attribute("width").Value);
                PropHeight = int.Parse(Prop.Attribute("height").Value);
                texWidth = int.Parse(Prop.Attribute("texWidth").Value);
                texHeight = int.Parse(Prop.Attribute("texHeight").Value);

                this.PropPrototypes[PropClass] = new Prop(PropTexture, PropWidth, PropHeight, texWidth, texHeight);
                this.PropPrototypes[PropClass].className = PropClass;

            }
            foreach (XElement Door in Doors)
            {
                DoorClass = Door.Attribute("className").Value;
                DoorTexture = content.Load<Texture2D>(Door.Attribute("texture").Value);
                DoorWidth = int.Parse(Door.Attribute("width").Value);
                DoorHeight = int.Parse(Door.Attribute("height").Value);
                texWidth = int.Parse(Door.Attribute("texWidth").Value);
                texHeight = int.Parse(Door.Attribute("texHeight").Value);

                this.DoorPrototypes[DoorClass] = new Door(DoorTexture, DoorWidth, DoorHeight, texWidth, texHeight);
                this.DoorPrototypes[DoorClass].className = DoorClass;
            }
            foreach (XElement Potion in Potions)
            {
                PotionClass = Potion.Attribute("className").Value;
                PotionTexture = content.Load<Texture2D>(Potion.Attribute("texture").Value);
                PotionWidth = int.Parse(Potion.Attribute("width").Value);
                PotionHeight = int.Parse(Potion.Attribute("height").Value);
                texWidth = int.Parse(Potion.Attribute("texWidth").Value);
                texHeight = int.Parse(Potion.Attribute("texHeight").Value);
                PotionRestore = int.Parse(Potion.Attribute("restore").Value);

                this.PotionPrototypes[PotionClass] = new Potion(PotionTexture, 
                                                                                            PotionWidth, PotionHeight,
                                                                                            texWidth, texHeight, PotionRestore);
                this.PotionPrototypes[PotionClass].className = PotionClass;
            }
       }

        public void spawnProp(Prop Prop, Point spawnTile, int spawnType)
        {
            //Prop.onSpawn(spawnTile, spawnType);
            //Prop.world = worldManager.curWorld;
        }

        public void spawnDoor(Door door, Point spawnTile, int spawnType)
        {
            door.onSpawn(spawnTile, spawnType);
            //Prop.world = worldManager.curWorld;
        }

        public void spawnPotion(Potion potion, Point spawnTile, int spawnType)
        {
            potion.onSpawn(spawnTile, spawnType);
        }

        public void spawnProps(List<SpawnFlag> spawns)
        {
            foreach (SpawnFlag spawn in spawns)
            {
                if (spawn.spawnType == (int)SPAWNTYPES.PROP || spawn.spawnType == (int)SPAWNTYPES.DOOR || spawn.spawnType == (int)SPAWNTYPES.POTION)
                {
                    if (PropPrototypes.ContainsKey(spawn.className))
                    {
                        Prop PropToCopy = (Prop)PropPrototypes[spawn.className].Clone();
                        Point spawnTile = spawn.tilePosition;

                        spawnProp(PropToCopy, spawnTile, spawn.spawnType);
                    }
                    else if(DoorPrototypes.ContainsKey(spawn.className))
                    {
                        Door DoorToCopy = (Door)DoorPrototypes[spawn.className].Clone();
                        Point spawnTile = spawn.tilePosition;

                        spawnDoor(DoorToCopy, spawnTile, spawn.spawnType);
                    }
                    else if (PotionPrototypes.ContainsKey(spawn.className))
                    {
                        Potion PotionToCopy = (Potion)PotionPrototypes[spawn.className].Clone();
                        Point spawnTile = spawn.tilePosition;

                        spawnPotion(PotionToCopy, spawnTile, spawn.spawnType);
                    }
                }
            }
        }

        private void World_Change(object sender, EventArgs args)
        {
            WorldManager worldManager = (WorldManager)sender;
            if (worldManager.curWorld != null)
            {
                List<SpawnFlag> spawns = worldManager.curWorld.getSpawns();
                spawnProps(spawns);
            }
        }
    }
}
