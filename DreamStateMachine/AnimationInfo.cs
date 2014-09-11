using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DreamStateMachine
{
    class AnimationInfo
    {
        public List<List<Point>> hitPoints;
        public String name;
        public int frames;
        public float fps;
        public int hitDamage;
        public int textureColumn;
        public int textureRow;

        public AnimationInfo(String name, int frames, float speed, int column, int row)
        {
            this.name = name;
            this.frames = frames;
            fps = speed;
            textureColumn = column;
            textureRow = row;
        }

    };
}
