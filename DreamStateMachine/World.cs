using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DreamStateMachine.Actions;

namespace DreamStateMachine
{
    class World
    {
        List<Room> roomList;
        List<SpawnFlag> spawns;
        Point topLeftTilePos;
        Point topRightTilePos;
        Point bottomLeftTilePos;
        Point spawnTile;
        Point bottomRightTilePos;
        Texture2D floorTiles;
        int[,] tileMap;
        bool[,] collisionMap;
        public int tileSize;

        public World(Texture2D floorTex, int ts)
        {
            floorTiles = floorTex;
            tileSize = ts;
            roomList = new List<Room>();
        }

        public List<Point> findPath(Point origin, Point target)
        {
            List<Point> newPath = new List<Point>();
            List<PathNode> openPoints = new List<PathNode>();
            List<PathNode> closedPoints = new List<PathNode>();
            origin.X /= tileSize;
            origin.Y /= tileSize;
            target.X /= tileSize;
            target.Y /= tileSize;

            if (origin.X != target.X && origin.Y != target.Y)
            {
                PathNode curNode = new PathNode(null, origin, target, 0);
                openPoints.Add(curNode);

                while (curNode.point.X != target.X || curNode.point.Y != target.Y)
                {
                    //if (tileMap[curNode.point.Y, curNode.point.X - 1] == 5 )
                    if (this.tileIsInBounds(curNode.point.X - 1, curNode.point.Y))
                    {
                        Point openPoint = new Point(curNode.point.X - 1, curNode.point.Y);
                        PathNode openPathNode = new PathNode(curNode, openPoint, target, curNode.movementCost + 10);
                        if (!openPoints.Contains(openPathNode) && !closedPoints.Contains(openPathNode))
                            openPoints.Insert(0, openPathNode);
                    }
                    //if (tileMap[curNode.point.Y - 1, curNode.point.X - 1] == 5 &&
                    //   tileMap[curNode.point.Y, curNode.point.X - 1] == 5 &&
                    //   tileMap[curNode.point.Y - 1, curNode.point.X] == 5)
                    if(this.tileIsInBounds(curNode.point.X - 1, curNode.point.Y - 1) &&
                        this.tileIsInBounds(curNode.point.X - 1, curNode.point.Y) &&
                        this.tileIsInBounds(curNode.point.X - 1, curNode.point.Y))
                    {
                        Point openPoint = new Point(curNode.point.X - 1, curNode.point.Y - 1);
                        PathNode openPathNode = new PathNode(curNode, openPoint, target, curNode.movementCost + 14);
                        if (!openPoints.Contains(openPathNode) && !closedPoints.Contains(openPathNode))
                            openPoints.Insert(0, openPathNode);
                    }
                    //if (tileMap[curNode.point.Y - 1, curNode.point.X] == 5)
                    if (this.tileIsInBounds(curNode.point.X, curNode.point.Y - 1))
                    {
                        Point openPoint = new Point(curNode.point.X, curNode.point.Y - 1);
                        PathNode openPathNode = new PathNode(curNode, openPoint, target, curNode.movementCost + 10);
                        if (!openPoints.Contains(openPathNode) && !closedPoints.Contains(openPathNode))
                            openPoints.Insert(0, openPathNode);
                    }
                    //if (tileMap[curNode.point.Y - 1, curNode.point.X + 1] == 5 &&
                    //   tileMap[curNode.point.Y, curNode.point.X + 1] == 5 &&
                    //   tileMap[curNode.point.Y - 1, curNode.point.X] == 5)
                    if (this.tileIsInBounds(curNode.point.X + 1, curNode.point.Y - 1) &&
                        this.tileIsInBounds(curNode.point.X + 1, curNode.point.Y) &&
                        this.tileIsInBounds(curNode.point.X, curNode.point.Y - 1))
                    {
                        Point openPoint = new Point(curNode.point.X + 1, curNode.point.Y - 1);
                        PathNode openPathNode = new PathNode(curNode, openPoint, target, curNode.movementCost + 14);
                        if (!openPoints.Contains(openPathNode) && !closedPoints.Contains(openPathNode))
                            openPoints.Insert(0, openPathNode);
                    }
                    //if (tileMap[curNode.point.Y, curNode.point.X + 1] == 5)
                    if (this.tileIsInBounds(curNode.point.X + 1, curNode.point.Y))
                    {
                        Point openPoint = new Point(curNode.point.X + 1, curNode.point.Y);
                        PathNode openPathNode = new PathNode(curNode, openPoint, target, curNode.movementCost + 10);
                        if (!openPoints.Contains(openPathNode) && !closedPoints.Contains(openPathNode))
                            openPoints.Insert(0, openPathNode);
                    }
                    //if (tileMap[curNode.point.Y + 1, curNode.point.X + 1] == 5 &&
                    //   tileMap[curNode.point.Y, curNode.point.X + 1] == 5 &&
                    //   tileMap[curNode.point.Y + 1, curNode.point.X] == 5)
                    if (this.tileIsInBounds(curNode.point.X + 1, curNode.point.Y + 1) &&
                        this.tileIsInBounds(curNode.point.X + 1, curNode.point.Y) &&
                        this.tileIsInBounds(curNode.point.X, curNode.point.Y + 1))
                    {
                        Point openPoint = new Point(curNode.point.X + 1, curNode.point.Y + 1);
                        PathNode openPathNode = new PathNode(curNode, openPoint, target, curNode.movementCost + 14);

                        if (!openPoints.Contains(openPathNode) && !closedPoints.Contains(openPathNode))
                            openPoints.Insert(0, openPathNode);
                    }
                    //if (tileMap[curNode.point.Y + 1, curNode.point.X] == 5)
                    if (this.tileIsInBounds(curNode.point.X, curNode.point.Y + 1))
                    {
                        Point openPoint = new Point(curNode.point.X, curNode.point.Y + 1);
                        PathNode openPathNode = new PathNode(curNode, openPoint, target, curNode.movementCost + 10);
                        if (!openPoints.Contains(openPathNode) && !closedPoints.Contains(openPathNode))
                            openPoints.Insert(0, openPathNode);
                    }
                    //if (tileMap[curNode.point.Y + 1, curNode.point.X - 1] == 5 &&
                    //   tileMap[curNode.point.Y, curNode.point.X - 1] == 5 &&
                    //   tileMap[curNode.point.Y + 1, curNode.point.X] == 5)
                    if (this.tileIsInBounds(curNode.point.X - 1, curNode.point.Y + 1) &&
                        this.tileIsInBounds(curNode.point.X - 1, curNode.point.Y) &&
                        this.tileIsInBounds(curNode.point.X, curNode.point.Y + 1))
                    {
                        Point openPoint = new Point(curNode.point.X - 1, curNode.point.Y + 1);
                        PathNode openPathNode = new PathNode(curNode, openPoint, target, curNode.movementCost + 14);
                        if (!openPoints.Contains(openPathNode) && !closedPoints.Contains(openPathNode))
                            openPoints.Insert(0, openPathNode);
                    }

                    PathNode cheapestNode = openPoints.ElementAt(0);

                    foreach (PathNode pN in openPoints)
                    {
                        if (pN.totalCost < cheapestNode.totalCost)
                        {
                            cheapestNode = pN;
                        }
                    }


                    closedPoints.Add(cheapestNode);
                    openPoints.Remove(cheapestNode);
                    curNode = cheapestNode;


                }

                //closedPoints.RemoveAt(0);

                while (curNode.parent != null)
                {
                    curNode.point.X = curNode.point.X * tileSize + tileSize / 2;
                    curNode.point.Y = curNode.point.Y * tileSize + tileSize / 2;
                    newPath.Insert(0, curNode.point);
                    curNode = curNode.parent;
                }
            }
            
            return newPath;

        }

        public bool isInBounds(int x, int y)
        {
            //int topLeftTilePosX = (int)(box.x / tileSize);
            //int topLeftTilePosY = (int)(box.y / tileSize);
            x /= tileSize;
            y /= tileSize;

            if (x >= 0 &&
                y >= 0 &&
                x < collisionMap.GetLength(1) &&
                y < collisionMap.GetLength(0) &&
                collisionMap[y, x])
                return true;
            else
                return false;
        }

        public bool isInBounds(Point point)
        {
            //int topLeftTilePosX = (int)(box.x / tileSize);
            //int topLeftTilePosY = (int)(box.y / tileSize);
            Point p = new Point(point.X, point.Y);
            p.X /= tileSize;
            p.Y /= tileSize;

            if (p.X >= 0 &&
                p.Y >= 0 &&
                p.X < collisionMap.GetLength(1) &&
                p.Y < collisionMap.GetLength(0) &&
                collisionMap[p.Y, p.X])
                return true;
            else
                return false;
        }

        public bool isInBounds(Rectangle box)
        {
            topLeftTilePos = new Point(box.Left / tileSize, box.Top / tileSize);
            topRightTilePos = new Point(box.Right / tileSize, box.Top / tileSize);
            bottomLeftTilePos = new Point(box.Left / tileSize, box.Bottom / tileSize);
            bottomRightTilePos = new Point(box.Right / tileSize, box.Bottom / tileSize);

            if (topLeftTilePos.X >= 0 && 
                topLeftTilePos.Y >= 0 &&
                bottomRightTilePos.X < collisionMap.GetLength(1) &&
                bottomRightTilePos.Y < collisionMap.GetLength(0) &&
                collisionMap[ topLeftTilePos.Y, topLeftTilePos.X] &&
                collisionMap[ topRightTilePos.Y, topRightTilePos.X] &&
                collisionMap[ bottomLeftTilePos.Y, bottomLeftTilePos.X] &&
                collisionMap[ bottomRightTilePos.Y, bottomRightTilePos.X])
                    return true;
            else
                    return false;
        }

        public bool isTileDrawable(int x, int y)
        {
            //int topLeftTilePosX = (int)(box.x / tileSize);
            //int topLeftTilePosY = (int)(box.y / tileSize);
            x /= tileSize;
            y /= tileSize;

            if (x >= 0 &&
                y >= 0 &&
                x < tileMap.GetLength(1) &&
                y < tileMap.GetLength(0) &&
                tileMap[y, x] != 0)
                return true;
            else
                return false;
        }

        public bool tileIsInBounds(int x, int y)
        {
            if (x >= 0 &&
                y >= 0 &&
                x < collisionMap.GetLength(1) &&
                y < collisionMap.GetLength(0) &&
                collisionMap[y, x])
                return true;
            else
                return false;
        }

        public bool isInSight(Actor actor, Point point)
        {
            double distance = Math.Sqrt(Math.Pow(point.X - actor.hitBox.Center.X, 2) + Math.Pow(point.Y - actor.hitBox.Center.Y, 2));
            if (distance < actor.sight)
            {
                Vector2 direction = new Vector2(point.X - actor.hitBox.Center.X, point.Y - actor.hitBox.Center.Y);
                direction.Normalize();

                double dotProduct = Vector2.Dot(direction, actor.sightVector);
                if (dotProduct > 0)
                {
                    TraceInfo traceInfo = this.traceWorld(point, actor.hitBox.Center);
                    if (!traceInfo.hitWorld)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public int[,] getTileMap()
        {
            return tileMap;
        }

        public Texture2D getFloorTileTex()
        {
            return this.floorTiles;
        }

        public int getTileSize()
        {
            return tileSize;
        }

        public Point getSpawnPos()
        {
            Point spawnTile = this.getSpawnTile();
            Point spawnPos = new Point(spawnTile.X * tileSize + tileSize / 2, spawnTile.Y * tileSize + tileSize / 2);
            return spawnPos;
        }

        public Point getSpawnTile()
        {
            return spawnTile;
        }

        public List<SpawnFlag> getSpawns()
        {
            return spawns;
        }

        public void printMap()
        {
            String temp = "";
            for (int i = 0; i < tileMap.GetLength(0); i++)
            {

                for (int k = 0; k < tileMap.GetLength(1); k++)
                {
                    if (tileMap[i, k] != -1 && tileMap[i, k] < 10)
                    {
                        temp = temp + tileMap[i, k] + " ";
                    }
                    else
                    {
                        temp = temp + tileMap[i, k];
                    }

                }
                temp = temp + "\n";
                Console.WriteLine(temp);
                temp = "";

            }
        }

        public void setTileMap(int[,] map, bool[,] cMap)
        {
            tileMap = map;
            collisionMap = cMap;
        }

        public void setTileSize(int s)
        {
           tileSize = s;
        }

        public void setSpawnTile(Point pos)
        {
            spawnTile = pos;
        }

        public void setSpawns(List<SpawnFlag> s)
        {
            spawns = s;
        }

        public void useTileAtPoint(Point point)
        {
            Point tilePos = new Point(point.X / this.tileSize, point.Y / this.tileSize);
            Console.WriteLine("Tile being used" + tileMap[tilePos.Y, tilePos.X]);
        }

        public void useTileAtPoint(int x, int y)
        {
            Point tilePos = new Point(x / this.tileSize, y / this.tileSize);
            Console.WriteLine("Tile being used" + tileMap[tilePos.Y, tilePos.X]);
        }

        public TraceInfo traceWorld(Point origin, Point destination)
        {
            TraceInfo traceInfo;
            Vector2 ray = new Vector2(destination.X - origin.X, destination.Y - origin.Y);
            float curRayLength = 0;
            float maxRayLength = ray.Length();
            ray.Normalize();
            ray *= tileSize;
            Vector2 curPoint = new Vector2(origin.X, origin.Y);
            while(((int)curPoint.X != (int)destination.X || (int)curPoint.Y != (int)destination.Y) && curRayLength < maxRayLength)
            {
                curPoint += ray;
                curRayLength += tileSize;
                if (!this.isInBounds((int)curPoint.X, (int)curPoint.Y))
                {
                    curPoint -= ray;
                    traceInfo = new TraceInfo(true, false, new Point((int)curPoint.X, (int)curPoint.Y));
                    return traceInfo;
                }
            }
                
            traceInfo = new TraceInfo(false, false);
            return traceInfo;

        }
    }
}
