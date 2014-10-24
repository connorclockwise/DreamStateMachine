using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Xml.Linq;
using DreamStateMachine;
using Microsoft.Xna.Framework.Media;

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
            if (usingActor.health > 0)
            {
                int reach = usingActor.reach;
                Vector2 sightVector = usingActor.sightVector;
                Point usePoint = new Point();
                usePoint.X = (int)((usingActor.hitBox.Center.X + (int)(sightVector.X * reach)) / this.curWorld.tileSize);
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
        }

        public void initWorldConfig(ContentManager content, String actorConfigFile)
        {
            XDocument doc = XDocument.Load(actorConfigFile);
            List<XElement> worlds = doc.Element("Worlds").Elements("World").ToList();
            List<XElement> enemies;

            String worldName;
            List<String> enemyClasses;
            Texture2D texture;
            int width;
            int height;
            int tileSize;

            foreach (XElement world in worlds)
            {
                worldName = world.Attribute("worldName").Value;
                enemies = world.Elements("Enemy").ToList();
                texture = content.Load<Texture2D>(world.Attribute("worldTexture").Value);
                width = int.Parse(world.Attribute("width").Value);
                height = int.Parse(world.Attribute("height").Value);
                tileSize = int.Parse(world.Attribute("tileSize").Value);
                enemyClasses = new List<String>();
                foreach (XElement enemy in enemies)
                {
                    enemyClasses.Add(enemy.Attribute("class").Value);
                }
                worldPrototypes[worldName] = new WorldConfig(worldName, enemyClasses, texture, width, height, tileSize);
                worldPrototypes[worldName].music = world.Attribute("themeMusic").Value;
            }
        }

        public void initStartingWorld()
        {
            curLevel = 1;
            curWorld = this.worldFactory.generateWorld(worldPrototypes["forest"], 5);
            Node<World> rootWorld = new Node<World>(curWorld);
            curWorldNode = rootWorld;
            worldTree.setRoot(rootWorld);
            SoundManager.Instance.playSong("templeTheme");
            //SoundManager.Instance.playSong(curWorld.themeMusic);
            
            onWorldChange();

        }

        public void initTutorial()
        {
            curLevel = 1;
            curWorld = loadTutorialLevel();
            curWorld.isTutorial = true;
            Node<World> rootWorld = new Node<World>(curWorld);
            curWorldNode = rootWorld;
            worldTree.setRoot(rootWorld);
            //SoundManager.Instance.playSong(curWorld.themeMusic);
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
            World prevWorld = curWorld;
            
            World tempWorld = this.worldFactory.generateWorld(worldPrototypes["nightmare"], 5);
            Node<World> tempNode = new Node<World>(tempWorld);
            curWorldNode.Children.Insert(worldIndex, tempNode);
            curWorldNode = curWorldNode.Children[worldIndex];
            curWorld = curWorldNode.Value;
            curLevel++;
            //if (curWorld.themeMusic != prevWorld.themeMusic)
            //{
            SoundManager.Instance.playSong("templeTheme");
            SoundManager.Instance.switchSong("templeTheme", curWorld.themeMusic);
                //SoundManager.Instance.stopSong(prevWorld.themeMusic);
            //}
            return tempWorld;
        }

        private void onWorldChange(){
            worldChange(this, EventArgs.Empty);
        }

        public World loadFromCustom(WorldConfig worldConfig, int[,] tileMap, bool[,] collisionMap, Point playerSpawnPos, List<String> enemyTypeList, List<Point> enemySpawnPosList)
        {
            return this.loadFromCustom(worldConfig.texture, worldConfig.tileSize, tileMap, collisionMap, playerSpawnPos, enemyTypeList, enemySpawnPosList);
        }

        public World loadFromCustom(Texture2D tile, int tileSize, int[,] tileMap, bool[,] collisionMap, Point playerSpawnPos, List<String> enemyTypeList, List<Point> enemySpawnPosList)
        {
            World tempWorld = new World(tile, tileSize);
            List<SpawnFlag> spawns = new List<SpawnFlag>();
            SpawnFlag playerSpawn = new SpawnFlag("player", playerSpawnPos, 1);
            tempWorld.setTileMap(tileMap, collisionMap);
            spawns.Add(playerSpawn);
            tempWorld.setSpawnTile(playerSpawnPos);
            SpawnFlag enemySpawn;
            if(enemyTypeList.Count == enemySpawnPosList.Count)
            {
                for(int i = 0; i < enemyTypeList.Count; i++)
                {
                    enemySpawn = new SpawnFlag(enemyTypeList.ElementAt(i), enemySpawnPosList.ElementAt(i), 2);
                    spawns.Add(enemySpawn);
                }
            }
            tempWorld.setSpawns(spawns);
            return tempWorld;

        }

        public World loadTutorialLevel()
        {
            int[,] tileMap = {{0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                              {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                              {0, 0, 1, 2, 2, 2, 3, 0, 0, 0, 0, 1, 2, 2, 2, 2, 3, 0, 0, 0, 0, 1, 2, 2, 2, 2, 2, 3, 0, 0},
                              {0, 0, 4, 5, 5, 5, 6, 0, 0, 0, 0, 4, 5, 5, 5, 5, 6, 0, 0, 0, 0, 4, 5, 5, 5, 5, 5, 6, 0, 0},
                              {0, 0, 4, 5,15, 5, 6, 0, 0, 0, 0, 4, 5, 5, 5, 5, 6, 0, 0, 0, 0, 4, 5, 5, 5, 5, 5, 6, 0, 0},
                              {0, 0, 4, 5, 5, 5, 6, 0, 0, 0, 0, 4, 5, 5, 5, 5,12, 2, 2, 2, 2,13, 5, 5, 5, 5, 5, 6, 0, 0},
                              {0, 0, 7,11, 5,10, 9, 0, 0, 0, 0, 4, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 6, 0, 0},
                              {0, 0, 0, 4, 5, 6, 0, 0, 0, 0, 0, 4, 5, 5, 5, 5,10, 8, 8, 8, 8,11, 5, 5, 5, 5, 5, 6, 0, 0},
                              {0, 0, 0, 4, 5, 6, 0, 0, 0, 0, 0, 4, 5, 5, 5, 5, 6, 0, 0, 0, 0, 4, 5, 5, 5, 5, 5, 6, 0, 0},
                              {0, 1, 2,13, 5,12, 2, 3, 0, 0, 0, 7,11, 5,10, 8, 9, 0, 0, 0, 0, 7, 8,11, 5,10, 8, 9, 0, 0},
                              {0, 4, 5, 5, 5, 5, 5, 6, 0, 0, 0, 0, 4, 5, 6, 0, 0, 0, 0, 0, 0, 0, 0, 4, 5, 6, 0, 0, 0, 0},
                              {0, 4, 5, 5, 5, 5, 5, 6, 0, 0, 1, 2,13, 5,12, 2, 3, 0, 0, 0, 0, 0, 0, 4, 5, 6, 0, 0, 0, 0},
                              {0, 4, 5, 5, 5, 5, 5, 6, 0, 0, 4, 5, 5, 5, 5, 5, 6, 0, 0, 0, 1, 2, 2,13, 5,12, 2, 3, 0, 0},
                              {0, 4, 5, 5, 5, 5, 5, 6, 0, 0, 4, 5, 5, 5, 5, 5, 6, 0, 0, 0, 4, 5, 5, 5, 5, 5, 5, 6, 0, 0},
                              {0, 4, 5, 5, 5, 5, 5, 6, 0, 0, 4, 5, 5, 5, 5, 5, 6, 0, 0, 0, 4, 5, 5, 5, 5, 5, 5, 6, 0, 0},
                              {0, 7, 8,11, 5,10, 8, 9, 0, 0, 4, 5, 5, 5, 5, 5, 6, 0, 0, 0, 4, 5, 5, 5, 5, 5, 5, 6, 0, 0},
                              {0, 0, 0, 4, 5, 6, 0, 0, 0, 0, 7, 8, 8, 8, 8, 8, 9, 0, 0, 0, 4, 5, 5, 5, 5, 5, 5, 6, 0, 0},
                              {0, 0, 0, 4, 5, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 7, 8, 8,11, 5,10, 8, 9, 0, 0},
                              {0, 0, 0, 4, 5, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 5, 6, 0, 0, 0, 0},
                              {0, 0, 0, 4, 5, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 5, 6, 0, 0, 0, 0},
                              {0, 1, 2,13, 5,12, 3, 0, 0, 0, 1, 2, 2, 2, 2, 2, 3, 0, 0, 0, 0, 0, 0, 4, 5, 6, 0, 0, 0, 0},
                              {0, 4, 5, 5, 5, 5, 6, 0, 0, 0, 4, 5, 5, 5, 5, 5, 6, 0, 0, 0, 1, 2, 2,13, 5,12, 3, 0, 0, 0},
                              {0, 4, 5, 5, 5, 5, 6, 0, 0, 0, 4, 5, 5, 5, 5, 5, 6, 0, 0, 0, 4, 5, 5, 5, 5, 5, 6, 0, 0, 0},
                              {0, 4, 5, 5, 5, 5,12, 2, 2, 2,13, 5, 5, 5, 5, 5,12, 2, 2, 2,13, 5, 5, 5, 5, 5, 6, 0, 0, 0},
                              {0, 4, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 6, 0, 0, 0},
                              {0, 4, 5, 5, 5, 5,10, 8, 8, 8,11, 5, 5, 5, 5, 5,10, 8, 8, 8,11, 5, 5, 5, 5, 5, 6, 0, 0, 0},
                              {0, 4, 5, 5, 5, 5, 6, 0, 0, 0, 7, 8, 8, 8, 8, 8, 9, 0, 0, 0, 4, 5, 5, 5, 5, 5, 6, 0, 0, 0},
                              {0, 7, 8, 8, 8, 8, 9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 5, 5, 5, 5, 5, 6, 0, 0, 0},
                              {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 7, 8, 8, 8, 8, 8, 9, 0, 0, 0},
                              {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}};
            bool[,] collisionMap = new bool[30,30];
            for(int i = 0; i < 30; i++) {
                for(int j = 0; j < 30; j++) {
                    collisionMap[i, j] = ((tileMap[i, j] == 5) || (tileMap[i, j] == 15));
                }
            }
            Point playerSpawnPos = new Point(15,14);
            List<String> enemyTypeList = new List<String>();
            enemyTypeList.Add("skeleton");
            List<Point> enemySpawnPosList = new List<Point>();
            enemySpawnPosList.Add(new Point(4, 13));
            return this.loadFromCustom(worldPrototypes["forest"], tileMap, collisionMap, playerSpawnPos, enemyTypeList, enemySpawnPosList);
        }

        public void restart()
        {
            curLevel = -1;
            curWorld = null;
            curWorldNode = null;
            worldChange(this, EventArgs.Empty);
        }
    }

}
