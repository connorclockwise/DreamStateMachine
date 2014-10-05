using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DreamStateMachine
{
    class Room
    {
        public Rectangle dimensions;
        public bool startRoom;
        //public List<Enemy> enemyList;
            
        public Room(Rectangle rect)
        {
            dimensions = rect;
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
