using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DreamStateMachine
{
    class AnimationInfo
    {
        public Dictionary<int, int> attackDamage;
        public Dictionary<int, List<Rectangle>> attackPoints;
        public String name;
        public String type;
        public int frames;
        public int fps;
        public int texColumn;
        public int texRow;

        public AnimationInfo(String name, int frames, int fps, int column, int row)
        {
            this.name = name;
            this.frames = frames;
            this.fps = fps;
            texColumn = column;
            texRow = row;
        }

        public AnimationInfo(String name, int frames, int fps, int column, int row, Dictionary<int, int> damage, Dictionary<int, List<Rectangle>> damagePoints)
        {
            attackDamage = damage;
            attackPoints = damagePoints;
            this.name = name;
            this.frames = frames;
            this.fps = fps;
            texColumn = column;
            texRow = row;
        }

    };
}
