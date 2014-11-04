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
        List<Prop> props;
        Dictionary<String, Prop> PropPrototypes;
        Dictionary<String, Door> DoorPrototypes;
        Dictionary<String, Potion> PotionPrototypes;
        Dictionary<String, Key> KeyPrototypes;
        Dictionary<String, WeaponItem> WeaponItemPrototypes;
        Random random;

        public PropManager()
        {
            props = new List<Prop>();
            PropPrototypes = new Dictionary<string,Prop>();
            DoorPrototypes = new Dictionary<string,Door>();
            PotionPrototypes = new Dictionary<string, Potion>();
            KeyPrototypes = new Dictionary<string, Key>();
            WeaponItemPrototypes = new Dictionary<string, WeaponItem>();
            random = new Random();

            Actor.Death += new EventHandler<EventArgs>(Actor_Death);
            Actor.Drop += new EventHandler<PickupEventArgs>(Actor_Drop);
            WorldManager.worldChange += new EventHandler<EventArgs>(World_Change);
        }

        public void initPropConfig(ContentManager content, String PropConfigFile)
        {
            var doc = XDocument.Load(PropConfigFile);
            var Props = doc.Element("Props").Elements("Prop");
            var Doors = doc.Element("Props").Elements("Door");
            var Potions = doc.Element("Props").Elements("Potion");
            var Keys = doc.Element("Props").Elements("Key");
            var WeaponItems = doc.Element("Props").Elements("Weapon");

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

            String KeyClass;
            Texture2D KeyTexture;
            int KeyWidth;
            int KeyHeight;

            String WeaponClass;
            Texture2D WeaponTexture;
            int WeaponX;
            int WeaponY;
            int WeaponWidth;
            int WeaponHeight;

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
            foreach (XElement Key in Keys)
            {
                KeyClass = Key.Attribute("className").Value;
                KeyTexture = content.Load<Texture2D>(Key.Attribute("texture").Value);
                KeyWidth = int.Parse(Key.Attribute("width").Value);
                KeyHeight = int.Parse(Key.Attribute("height").Value);
                texWidth = int.Parse(Key.Attribute("texWidth").Value);
                texHeight = int.Parse(Key.Attribute("texHeight").Value);

                this.KeyPrototypes[KeyClass] = new Key(KeyTexture,
                                                        KeyWidth, KeyHeight,
                                                        texWidth, texHeight);
                this.KeyPrototypes[KeyClass].className = KeyClass;
            }
            foreach (XElement Weapon in WeaponItems)
            {
                WeaponClass = Weapon.Attribute("className").Value;
                WeaponTexture = content.Load<Texture2D>(Weapon.Attribute("texture").Value);
                WeaponX = int.Parse(Weapon.Attribute("offsetX").Value);
                WeaponY = int.Parse(Weapon.Attribute("offsetY").Value);
                WeaponWidth = int.Parse(Weapon.Attribute("width").Value);
                WeaponHeight = int.Parse(Weapon.Attribute("height").Value);
                texWidth = int.Parse(Weapon.Attribute("texWidth").Value);
                texHeight = int.Parse(Weapon.Attribute("texHeight").Value);

                this.WeaponItemPrototypes[WeaponClass] = new WeaponItem(WeaponTexture,WeaponClass,
                                                        WeaponWidth, WeaponHeight,
                                                        WeaponX, WeaponY,
                                                        texWidth, texHeight);
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

        public void spawnKey(Key key, Point spawnTile, int spawnType)
        {
            key.onSpawn(spawnTile, spawnType);
        }

        public void spawnWeapon(WeaponItem weapon, Point spawnTile, int spawnType)
        {
            weapon.onSpawn(spawnTile, spawnType);
        }

        public void spawnProps(List<SpawnFlag> spawns)
        {
            foreach (SpawnFlag spawn in spawns)
            {
                if (spawn.spawnType == (int)SPAWNTYPES.PROP || spawn.spawnType == (int)SPAWNTYPES.DOOR || spawn.spawnType == (int)SPAWNTYPES.POTION || spawn.spawnType == (int)SPAWNTYPES.KEY)
                {
                    if (PropPrototypes.ContainsKey(spawn.className))
                    {
                        Prop PropToCopy = (Prop)PropPrototypes[spawn.className].Clone();
                        Point spawnTile = spawn.tilePosition;

                        spawnProp(PropToCopy, spawnTile, spawn.spawnType);
                        props.Add(PropToCopy);
                    }
                    else if(DoorPrototypes.ContainsKey(spawn.className))
                    {
                        Door DoorToCopy = (Door)DoorPrototypes[spawn.className].Clone();
                        Point spawnTile = spawn.tilePosition;

                        spawnDoor(DoorToCopy, spawnTile, spawn.spawnType);
                        props.Add(DoorToCopy);
                    }
                    else if (PotionPrototypes.ContainsKey(spawn.className))
                    {
                        Potion PotionToCopy = (Potion)PotionPrototypes[spawn.className].Clone();
                        Point spawnTile = spawn.tilePosition;

                        spawnPotion(PotionToCopy, spawnTile, spawn.spawnType);
                        props.Add(PotionToCopy);
                    }
                    else if (KeyPrototypes.ContainsKey(spawn.className))
                    {
                        Key KeyToCopy = (Key)KeyPrototypes[spawn.className].Clone();
                        Point spawnTile = spawn.tilePosition;

                        spawnKey(KeyToCopy, spawnTile, spawn.spawnType);
                        props.Add(KeyToCopy);
                    }
                }
            }
        }

        public void update(float dt)
        {
        }

        private void Actor_Death(object sender, EventArgs args)
        {
            Actor deadActor = (Actor)sender;
            Point pointOfDeath = new Point();
            pointOfDeath.X = deadActor.hitBox.Center.X / deadActor.world.tileSize;
            pointOfDeath.Y = deadActor.hitBox.Center.Y / deadActor.world.tileSize;
            if (deadActor.hasKey)
            {
                spawnKey(KeyPrototypes["key"], pointOfDeath, 0);
            }
            foreach (KeyValuePair<String, float> entry in deadActor.lootTable)
            {
                List<Point> dropPoints = new List<Point>();
                dropPoints.Add( new Point(pointOfDeath.X - 1, pointOfDeath.Y - 1));
                dropPoints.Add( new Point(pointOfDeath.X, pointOfDeath.Y - 1));
                dropPoints.Add( new Point(pointOfDeath.X + 1, pointOfDeath.Y - 1));
                dropPoints.Add( new Point(pointOfDeath.X - 1, pointOfDeath.Y));
                dropPoints.Add( new Point(pointOfDeath.X + 1, pointOfDeath.Y));
                dropPoints.Add( new Point(pointOfDeath.X - 1, pointOfDeath.Y + 1));
                dropPoints.Add( new Point(pointOfDeath.X , pointOfDeath.Y + 1));
                dropPoints.Add( new Point(pointOfDeath.X + 1, pointOfDeath.Y + 1));
                float value = (float)random.NextDouble();
                if (value <= entry.Value)
                {
                    Point newDropPoint = dropPoints.Find(tile => deadActor.world.tileIsInBounds(tile.X, tile.Y));
                    spawnWeapon(WeaponItemPrototypes[entry.Key], newDropPoint, (int)SPAWNTYPES.ITEM);
                    dropPoints.Remove(newDropPoint);
                }
            }
        }

        private void Actor_Drop(object sender, PickupEventArgs args)
        {
            Actor droppingActor = (Actor)sender;
            Point pointOfDrop = new Point();
            pointOfDrop.X = droppingActor.hitBox.Center.X / droppingActor.world.tileSize;
            pointOfDrop.Y = droppingActor.hitBox.Center.Y / droppingActor.world.tileSize;
            spawnWeapon(WeaponItemPrototypes[args.itemClassName], pointOfDrop, (int)SPAWNTYPES.ITEM);
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
