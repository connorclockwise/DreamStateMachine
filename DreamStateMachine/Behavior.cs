using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DreamStateMachine
{
    abstract class Behavior:Action
    {
        public List<Point> path;
        public Point destination;
        public Point origin;
        public Point nextPathPoint;
    };
}
