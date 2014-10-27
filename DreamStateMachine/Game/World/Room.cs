using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DreamStateMachine
{
    class Room
    {
        public List<SpawnFlag> spawns;
        public List<Room> children;
        public Point hallEntrance;
        public Rectangle dimensions;
        public bool startRoom;
        public bool isLeaf;
        public bool isOptional;
        public Room parent;
        public int depth;
        //public List<Enemy> enemyList;
            
        public Room(Rectangle rect)
        {
            dimensions = rect;
            spawns = new List<SpawnFlag>();
            children = new List<Room>();
        }

        public Room(Rectangle rect, bool start)
        {
            dimensions = rect;
            startRoom = start;
        }

        //public int[] getDimensions()
        //{
        //    int[] dimensions = new int[4];
        //    dimensions[0] = rectangle.X;
        //    dimensions[1] = rectangle.Y;
        //    dimensions[2] = rectangle.Width;
        //    dimensions[3] = rectangle.Height;
        //    return dimensions;
        //}

    }
}
