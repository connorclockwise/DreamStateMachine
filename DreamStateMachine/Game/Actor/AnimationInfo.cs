using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DreamStateMachine
{
    public struct FrameInfo
    {
        public int attackDamage;
        public List<Rectangle> attackPoints;
        public Point gripPoint;
        public String stance;
        public float rotation;
    }

    public struct AnimationInfo
    {
        public FrameInfo[] frames;
        public String name;
        public String type;
        public int frameCount;
        public int fps;
        public int texColumn;
        public int texRow;
    };
}
