using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DreamStateMachine
{
    class PathNode : IEquatable<PathNode>
    {
        public PathNode parent;
        public Point point;
        public int movementCost;
        private int heuristicCost;
        public int totalCost;

        public PathNode(PathNode par, Point p, Point target, int g)
        {
            parent = par;
            point = p;
            movementCost = g;
            heuristicCost = 10 * (Math.Abs(target.X - point.X) + Math.Abs(target.Y - point.Y));
            totalCost = movementCost + heuristicCost;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is PathNode))
                return false;
            else
            {
                PathNode tempPathNode = (PathNode)obj;
                return tempPathNode.point.X == point.X && tempPathNode.point.Y == point.Y;
            }
        }

        public bool Equals(PathNode node)
        {
            PathNode tempPathNode = (PathNode)node;
            return tempPathNode.point.X == point.X && tempPathNode.point.Y == point.Y;
        }


    }
}
