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
        Random random;

        public PropManager()
        {
            PropPrototypes = new Dictionary<string,Prop>();
            random = new Random();
        }

        public void initPropConfig(ContentManager content, String PropConfigFile)
        {
            var doc = XDocument.Load(PropConfigFile);
            var Props = doc.Element("Props").Elements("Prop");
            String PropClass;
            Texture2D PropTexture;
            int PropWidth;
            int PropHeight;

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
       }

        public void spawnProp(Prop Prop, Point spawnTile, int spawnType)
        {
            Prop.onSpawn(spawnTile, spawnType);
            //Prop.world = worldManager.curWorld;
        }

        public void spawnProps(List<SpawnFlag> spawns)
        {
            foreach (SpawnFlag spawn in spawns)
            {
                if (PropPrototypes.ContainsKey(spawn.className))
                {
                        Prop PropToCopy = (Prop)PropPrototypes[spawn.className].Clone();
                        Point spawnTile = spawn.tilePosition;

                        spawnProp(PropToCopy, spawnTile, spawn.spawnType);
                }
                else if (spawn.className == "player_spawn")
                {
                    //Prop protagonist = new Prop();

                }

                
            }
        }
    }
}
