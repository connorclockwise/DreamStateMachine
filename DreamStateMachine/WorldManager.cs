using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Xml.Linq;
using DreamStateMachine;

namespace DreamStateMachine
{
    class WorldManager
    {
        public static event EventHandler<EventArgs> worldChange;

        public Node<World> curWorldNode;
        public World curWorld;
        public int curLevel;

        private Dictionary<String, WorldConfig> worldPrototypes;
        private Random random;
        private Tree<World> worldTree;
        private WorldFactory worldFactory;

        //Constructor
        public WorldManager(Random r)
        {
            worldPrototypes = new Dictionary<string, WorldConfig>();
            random = r;
            worldFactory = new WorldFactory(r);
            worldTree = new Tree<World>();
            curLevel = 1;

            Actor.Use += new EventHandler<EventArgs>(Actor_Use);
            Actor.Spawn += new EventHandler<SpawnEventArgs>(Actor_Spawn);
        }

        private void Actor_Spawn(object sender, SpawnEventArgs e)
        {
            Actor spawnedActor = (Actor)sender;
            spawnedActor.world = curWorld;
        }

        private void Actor_Use(object sender, EventArgs e)
        {
            
            Actor usingActor = (Actor)sender;
            int reach = usingActor.reach;
            Vector2 sightVector = usingActor.sightVector;
            Point usePoint = new Point();
            usePoint.X = (int)((usingActor.hitBox.Center.X + (int)(sightVector.X * reach))  / this.curWorld.tileSize);
            usePoint.Y = (int)((usingActor.hitBox.Center.Y + (int)(sightVector.Y * reach)) / this.curWorld.tileSize);
            int[,] tileMap = this.curWorld.getTileMap();
            if (tileMap[usePoint.Y, usePoint.X] == 15)
            {
                //worldManager.curWorld.useTileAtPoint(usePoint);
                //Console.Write(worldManager.curLevel);
                if (this.getWorldChild(0) == null)
                {
                    this.createNextWorld(0);
                    onWorldChange();
                    
                    //this.spawnActor(protagonist, worldManager.curWorld.getSpawnPos(), 1);
                    //this.spawnActors(worldManager.curWorld.getSpawns());
                }
                //isLoadingWorld = false;
                //Console.Write(worldManager.curLevel);
            }
        }

        public void initWorldConfig(ContentManager content, String actorConfigFile)
        {
            var doc = XDocument.Load(actorConfigFile);
            var worlds = doc.Element("Worlds").Elements("World");

            String worldName;
            Texture2D texture;
            int width;
            int height;
            int tileSize;

            foreach (XElement world in worlds)
            {
                worldName = world.Attribute("worldName").Value;
                texture = content.Load<Texture2D>(world.Attribute("worldTexture").Value);
                width = int.Parse(world.Attribute("width").Value);
                height = int.Parse(world.Attribute("height").Value);
                tileSize = int.Parse(world.Attribute("tileSize").Value);
                worldPrototypes[worldName] = new WorldConfig(worldName, texture, width, height, tileSize);
            }
        }

        public void initStartingWorld()
        {
            curLevel = 1;
            curWorld = this.worldFactory.generateWorld(worldPrototypes["temple"], 5);
            Node<World> rootWorld = new Node<World>(curWorld);
            curWorldNode = rootWorld;
            //Node<World> newWorldNode = Node<World>(curWorld);
            worldTree.setRoot(rootWorld);
            onWorldChange();
        }

        public World getCurWorld()
        {
            return curWorld;
        }

        public World getWorldChild(int worldIndex)
        {
            if (curWorldNode.Children != null)
            {
                if (worldIndex < curWorldNode.Children.Count -1 && curWorldNode.Children[worldIndex] != null)
                {
                    if (curWorldNode.Children[worldIndex].Value != null)
                    {
                        return curWorldNode.Children[worldIndex].Value;
                    }
                }
            }
            return null;
        }

        public World createNextWorld(int worldIndex)
        {
            World tempWorld = this.worldFactory.generateWorld(worldPrototypes["tundra"], 5);
            Node<World> tempNode = new Node<World>(tempWorld);
            curWorldNode.Children.Insert(worldIndex, tempNode);
            curWorldNode = curWorldNode.Children[worldIndex];
            curWorld = curWorldNode.Value;
            curLevel++;
            return tempWorld;
        }

        private void onWorldChange(){
            worldChange(this, EventArgs.Empty);
        }

    }

}
