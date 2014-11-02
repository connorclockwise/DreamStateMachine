using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Xml.Linq;
using DreamStateMachine2.game.World;

namespace DreamStateMachine
{
    class WorldConfig
    {
        public String worldName;
        public List<EnemyConfig> enemyConfigs;
        public Texture2D texture;
        public int width;
        public int height;
        public int tileSize;
        public String music;

        //Constructor
        public WorldConfig(String worldName, List<EnemyConfig> enemyConfigs, Texture2D worldTexture, int worldWidth, int worldHeight, int tileSize)
        {
            this.worldName = worldName;
            this.enemyConfigs = enemyConfigs;
            this.texture = worldTexture;
            this.width = worldWidth;
            this.height = worldHeight;
            this.tileSize = tileSize;
        }
    }

}
