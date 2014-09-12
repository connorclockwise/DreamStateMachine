using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace DreamStateMachine
{
    class WorldFactory
    {
        Random random;
        World newWorld;
        List<Room> rooms;
        List<SpawnFlag> spawns;
        int[,] tileMap;
        bool[,] collisionMap;
        int remainingEnemies;
        int maxHallLength = 12;
        int maxRoomWidth = 8;
        int maxRoomHeight = 8;
        int maxRooms = 5;
        int minHallLength = 4;
        int minRoomWidth = 4;
        int minRoomHeight = 4;
        
        //Constructor
        public WorldFactory(Random r)
        {
            random = r;
            //roomMap = new int [w,h];
            //tileMap = new int [w,h];
        }


        //Checks if the given coordinate can house a room
        //within the given constraints. (maxRoomWidth, 
        //minRoomHeight, ect.)
        //Returns whether a room can fit or not
        public bool canRoomFit(int[,] map, int xCoor, int yCoor)
        {

            //Console.WriteLine("xCoor" + xCoor);
            //Console.WriteLine("yCoor" + yCoor);
            int stretchTall = 0;
            int stretchWide = 0;

            for (int i = 0; i <= minRoomWidth; i++)
            {
                //Console.WriteLine("Left:" + (xCoor - i));
                //Console.WriteLine("Right:" + (xCoor + i));
                if (xCoor - i <= 1 || (xCoor + i) >= map.GetLength(1))
                    return false;
                if (map[yCoor, xCoor - i] != 0)
                    return false;
                if (map[yCoor, xCoor + i] != 0)
                    return false;

                stretchWide = i;
            }
            for (int i = 0; i <= minRoomHeight; i++)
            {
                //Console.WriteLine("Up:" + (yCoor - i));
                //Console.WriteLine("Down:" + (yCoor + i));
                if (yCoor - i <= 1 || (yCoor + i) >= map.GetLength(0))
                    return false;
                if (map[yCoor - i, xCoor] != 0)
                    return false;
                if (map[yCoor + i, xCoor] != 0)
                    return false;

                stretchTall = i;
            }
            if (map[yCoor - stretchTall, xCoor - stretchWide] != 0 || map[yCoor - stretchTall, xCoor + stretchWide] != 0)
                return false;
            else if (map[yCoor + stretchTall, xCoor - stretchWide] != 0 || map[yCoor + stretchTall, xCoor + stretchWide] != 0)
                return false;

            //for (int i = 0; i <= minRoomWidth || i <= minRoomHeight; i++)
            //{
            //    Console.WriteLine(i);
            //    if (stretchWide == 0)
            //    {
            //        if (i < minRoomWidth)
            //        {
            //            //Console.WriteLine("Left:" + (xCoor - i));
            //            //Console.WriteLine("Right:" + (xCoor + i));
            //            if (xCoor - i >= 1 || (xCoor + i) <= map.GetLength(1))
            //                return false;
            //            if (map[yCoor, xCoor - i] != 0 || map[yCoor, xCoor + i] != 0)
            //                return false;
            //        }
            //        else if (i == minRoomWidth)
            //        {
            //            stretchWide = minRoomWidth;
            //        }
            //    }
                
            //    if (stretchTall != 0)
            //    {
            //        if (map[yCoor - stretchTall, xCoor - i] != 0 || map[yCoor - stretchTall, xCoor + i] != 0)
            //            return false;
            //        else if (map[yCoor + stretchTall, xCoor - i] != 0 || map[yCoor + stretchTall, xCoor + i] != 0)
            //            return false;
            //    }

            //    if (stretchTall == 0)
            //    {
            //        if (i < minRoomHeight)
            //        {
            //            //Console.WriteLine("Up:" + (yCoor - i));
            //            //Console.WriteLine("Down:" + (yCoor + i));
            //            if (yCoor - i >= 1 || (yCoor + i) <= map.GetLength(0))
            //                return false;                       
            //            if (map[yCoor - i, xCoor] != 0 || map[yCoor + i, xCoor] != 0)
            //                return false;
            //        }
            //        else if (i == minRoomHeight)
            //        {
            //            stretchTall = minRoomHeight;
            //        }
            //    }

            //    if (stretchWide != 0)
            //    {
            //        if (map[yCoor - i, xCoor - stretchWide] != 0 || map[yCoor - i, xCoor + stretchWide] != 0)
            //            return false;
            //        else if (map[yCoor + i, xCoor - stretchWide] != 0 || map[yCoor + i, xCoor + stretchWide] != 0)
            //            return false;
            //    }

            //}

            //Console.WriteLine("Room fits");
            return true;
        }

        //Finds an acceptable x,y position to house a room in the given tilemap.
        //Uses canRoomFit
        //Returns array of x,y coordinates of the new room site
        public Point chooseRoomSite(int[,] map, Rectangle dimensions){

            Point coors;

            coors.X = (int)random.Next(dimensions.X, dimensions.Width);
            coors.Y = (int)random.Next(dimensions.Y, dimensions.Height);

            while (map[coors.Y, coors.X] != 0 || (!canRoomFit(map, coors.X, coors.Y)))
            {
                coors.X = (int)random.Next(dimensions.X, dimensions.Width);
                coors.Y = (int)random.Next(dimensions.Y, dimensions.Height);
            }

            return coors;
        }

        //Find an acceptable x,y position within the given bounds to house a hall in the given tilemap.
        //Also uses force as an optional argument, where the method will search for a hall until it finds one, ignoring attempt number.
        //Returns array of x,y coorindates, direction, and length of the hall.
        //(Note direction returned is cardinal, 0 is north, 1 is east, 2 is south, and 3 is west.)
        public int[] chooseHallSite(int[,] map, Rectangle dimensions, bool force = false)
        {

            int maxAttempts;

            if (force)
                maxAttempts = 1000000000;
            else
                maxAttempts = 100;

            int attempts = 0;

            int[] coors = new int[6];
            int[] hallEnd = new int[2];
            int xSeed = dimensions.X;
            int ySeed = dimensions.Y;

            int direction;
            int idealHallLength = random.Next(minHallLength, maxHallLength + 1);
            int hallLength;
            int nextTile;
            
            do 
            {
                hallLength = 0;
                nextTile = 0;
                attempts++;
                do
                {
                    direction = random.Next(0, 4);
                    //Console.WriteLine("Im stuck here");
                    //Console.WriteLine("X:" + xSeed);
                    //Console.WriteLine("Y:" + ySeed);
                    switch (direction)
                    {

                        case 0:
                            xSeed = random.Next(dimensions.X + 1, dimensions.X + dimensions.Width - 1);
                            ySeed = dimensions.Y;
                            break;
                        case 1:
                            xSeed = dimensions.X + dimensions.Width - 1;
                            ySeed = random.Next(dimensions.Y + 1, dimensions.Y + dimensions.Height - 1);
                            break;
                        case 2:
                            xSeed = random.Next(dimensions.X + 1, dimensions.X + dimensions.Width - 1);
                            ySeed = dimensions.Y + dimensions.Height - 1;
                            break;
                        case 3:
                            xSeed = dimensions.X;
                            ySeed = random.Next(dimensions.Y + 1, dimensions.Y + dimensions.Height - 1);
                            break;
                    }
                } while (map[ySeed, xSeed] == 0 || map[ySeed, xSeed] == 1 || map[ySeed, xSeed] == 3 || map[ySeed, xSeed] == 5 || map[ySeed, xSeed] == 7 || map[ySeed, xSeed] == 9 );

                while (nextTile != -1 && hallLength < idealHallLength)
                {   
                    hallLength++;
                    switch (direction)
                    {
                        case 0:
                            nextTile = tileMap[ySeed - hallLength, xSeed];
                            hallEnd[0] = xSeed;
                            hallEnd[1] = ySeed - hallLength;
                            break;
                        case 1:
                            nextTile = tileMap[ySeed, xSeed + hallLength];
                            hallEnd[0] = xSeed + hallLength;
                            hallEnd[1] = ySeed;
                            break;
                        case 2:
                            nextTile = tileMap[ySeed + hallLength, xSeed];
                            hallEnd[0] = xSeed;
                            hallEnd[1] = ySeed + hallLength;
                            break;
                        case 3:
                            nextTile = tileMap[ySeed, xSeed - hallLength];
                            hallEnd[0] = xSeed - hallLength;
                            hallEnd[1] = ySeed;
                            break;
                    }
                }
            } while ((hallLength < minHallLength || hallLength > maxHallLength || !canRoomFit(map, hallEnd[0], hallEnd[1])) && (attempts < maxAttempts));

            //Console.WriteLine("Attempts:" + attempts);
            //Console.WriteLine("Can the room fit" + canRoomFit(map, hallEnd[0], hallEnd[1]));

            //Console.WriteLine("Hall X:" + xSeed);
            //Console.WriteLine("Hall Y:" + ySeed);
            //Console.WriteLine("Hall End X:" + hallEnd[0]);
            //Console.WriteLine("Hall End Y:" + hallEnd[1]);

            if (attempts >= maxAttempts)
                coors[0] = -1;
            else
            {
                coors[0] = xSeed;
                coors[1] = ySeed;
                coors[2] = direction;
                coors[3] = hallLength;
                coors[4] = hallEnd[0];
                coors[5] = hallEnd[1];
            }
            return coors;
        } 

        //Generates an World object with the given bounds using constraints outlined
        //at the top of the class.
        //Returns an array of enumerated tiles.

        public World generateWorld(WorldConfig worldConfig, int numEnemies)
        {
            return generateWorld(worldConfig.texture, worldConfig.width, worldConfig.height, worldConfig.tileSize, numEnemies);
        }

        public World generateWorld(Texture2D floorTex, int width, int height, int tileSize, int numEnemies)
        {
            rooms = new List<Room>();
            spawns = new List<SpawnFlag>();
            newWorld = new World(floorTex, tileSize);
            tileMap = new int[height, width];
            collisionMap = new bool[height, width];
            remainingEnemies = numEnemies;

            for (int i = 0; i < tileMap.GetLength(0); i++)
            {
                tileMap[i, 0] = -1;
                tileMap[i, width - 1] = -1;
            }

            for (int i = 0; i < tileMap.GetLength(1); i++)
            {
                tileMap[0, i] = -1;
                tileMap[height - 1, i] = -1;
            }

            Rectangle validWorldSpace = new Rectangle(1, 1, width - 1, height - 1);
            Point coors = chooseRoomSite(tileMap, validWorldSpace);
            Room firstRoom = placeRoom(tileMap, collisionMap, coors.X, coors.Y);
            firstRoom.startRoom = true;

            Point spawnPos = new Point(coors.X, coors.Y);
            SpawnFlag playerSpawn = new SpawnFlag("player_spawn", spawnPos, 1);
            spawns.Add(playerSpawn);
            newWorld.setSpawnTile(spawnPos);
            tileMap[coors.Y, coors.X] = 14;

            rooms.Add(firstRoom);

            int roomIndex = 0;
            int numRooms = 1;
            Room curRoom = rooms.ElementAt(roomIndex);
            while(numRooms < maxRooms)
            {
                int[] hallCoors;

                //Console.WriteLine("Current room index" + roomIndex);

                //Console.WriteLine("Room#" + numRooms);
                //if(numRooms > 1)
                //    hallCoors = chooseHallSite(tileMap, curRoom.dimensions);
                //else
                hallCoors = chooseHallSite(tileMap, curRoom.dimensions);

                if (hallCoors[0] == -1)
                {
                    //Console.WriteLine("Couldn't find a good hall site");
                    
                    if (roomIndex == 0)
                        roomIndex = random.Next(0, rooms.Count);
                    else
                        roomIndex--;

                    curRoom = rooms.ElementAt(roomIndex);
                }
                else
                {
                    if (random.Next(0, 4) == 0)
                    {
                        Room splitRoom = placeRoom(tileMap, collisionMap, hallCoors[4], hallCoors[5]);
                        rooms.Add(splitRoom);
                    }
                    else
                    {
                        curRoom = placeRoom(tileMap, collisionMap, hallCoors[4], hallCoors[5]);
                        rooms.Add(curRoom);
                    }
                    
                    placeHall(tileMap, collisionMap, hallCoors[0], hallCoors[1], hallCoors[2], hallCoors[3]);
                    roomIndex++;
                    numRooms++;
                }
            }

            Room room;
            for (int i = 0; i < rooms.Count; i++)
            {
                room = rooms.ElementAt(i);
                if (i != 0)
                {
                    placeEnemy(collisionMap, room, spawns);
                }
                if (i == rooms.Count - 1)
                {
                    tileMap[room.dimensions.Y + room.dimensions.Height / 2, room.dimensions.X + room.dimensions.Width / 2] = 15;
                }
            }

            //Set room list here
            newWorld.setTileMap(tileMap, collisionMap);
            newWorld.setSpawns(spawns);
            return newWorld;
        }


        public void placeEnemy(bool[,] cMap, Room room, List<SpawnFlag> spawns)
        {
            Rectangle dimensions = room.dimensions;
            Point coors;

            coors.X = (int)random.Next(dimensions.X + 1, dimensions.X + dimensions.Width - 2);
            coors.Y = (int)random.Next(dimensions.Y + 1, dimensions.Y + dimensions.Height - 2);

            SpawnFlag curSpawn;
            for (int i = 0; i < spawns.Count; i++ )
            {
                curSpawn = spawns[0];
                if (curSpawn.tilePosition.X == coors.X && curSpawn.tilePosition.Y == coors.Y)
                {
                    coors.X = (int)random.Next(dimensions.X + 1, dimensions.X + dimensions.Width - 2);
                    coors.Y = (int)random.Next(dimensions.Y + 1, dimensions.Y + dimensions.Height - 2);
                }
            }

            SpawnFlag spawnFlag = new SpawnFlag("skeleton", coors, 2);
            spawns.Add(spawnFlag);
        }

        //Places a hall in the given tilemap with given x,y coordinates, direction and length.
        //Direction is cardinal, 0 is north, 1 is east, 2 is south, 3 is west.)
        public void placeHall(int[,] map, bool[,] cMap, int xCoor, int yCoor, int direction, int length)
        {
            //Console.WriteLine("X:" + xCoor);
            //Console.WriteLine("Y:" + yCoor);
            //Console.WriteLine("Direction:" + direction);
            //Console.WriteLine("Length:" + length);
            for (int i = 0; i < length; i++)
            {

                switch (direction)
                {
                    case 0:
                        map[yCoor - i, xCoor] = 5;
                        cMap[yCoor - i, xCoor] = true;

                        if (map[yCoor - i, xCoor - 1] == 0 || map[yCoor - i, xCoor - 1] == 1 || map[yCoor - i, xCoor - 1] == 7)
                            map[yCoor - i, xCoor - 1] = 4;
                        else if (i == 0 && map[yCoor - i, xCoor - 1] == 2)
                            map[yCoor - i, xCoor - 1] = 13;
                        else if (map[yCoor -  i, xCoor - 1] == 8)
                            map[yCoor - i, xCoor - 1] = 11;


                        if (map[yCoor - i, xCoor + 1] == 0 || map[yCoor - i, xCoor + 1] == 3 || map[yCoor - i, xCoor + 1] == 9)
                            map[yCoor - i, xCoor + 1] = 6;
                        else if (i == 0 && map[yCoor - i, xCoor + 1] == 2)
                            map[yCoor - i, xCoor + 1] = 12;
                        else if (map[yCoor - i, xCoor + 1] == 8)
                            map[yCoor - i, xCoor + 1] = 10;

                        if(((i + 1) < length) && (map[yCoor - i - 1, xCoor] == 5))
                            i = length;

                        break;
                    case 1:
                        map[yCoor, xCoor + i] = 5;
                        cMap[yCoor, xCoor + i] = true;

                        if (map[yCoor - 1, xCoor + i] == 0 || map[yCoor - 1, xCoor + i] == 1 || map[yCoor - 1, xCoor + i] == 3)
                            map[yCoor - 1, xCoor + i] = 2;
                        else if (i == 0 && map[yCoor - 1, xCoor + i] == 6)
                            map[yCoor - 1, xCoor + i] = 12;
                        else if (map[yCoor - 1, xCoor + i] == 4)
                            map[yCoor - 1, xCoor + i] = 13;

                        if (map[yCoor + 1, xCoor + i] == 0 || map[yCoor + 1, xCoor + i] == 7 || map[yCoor + 1, xCoor + i] == 9)  
                            map[yCoor + 1, xCoor + i] = 8;
                        else if (i == 0 && map[yCoor + 1, xCoor + i] == 6)
                            map[yCoor + 1, xCoor + i] = 10;
                        else if (map[yCoor + 1, xCoor + i] == 4)
                            map[yCoor + 1, xCoor + i] = 11;

                        if (((i + 1) < length) && (map[yCoor, xCoor + i + 1] == 5))
                            i = length;

                        break;
                    case 2:
                        map[yCoor + i, xCoor] = 5;
                        cMap[yCoor + i, xCoor] = true;

                        if (map[yCoor + i, xCoor - 1] == 0 || map[yCoor + i, xCoor - 1] == 7 || map[yCoor + i, xCoor - 1] == 1)
                            map[yCoor + i, xCoor - 1] = 4;
                        else if (i == 0 && map[yCoor + i, xCoor - 1] == 8)
                            map[yCoor + i, xCoor - 1] = 11;
                        else if (map[yCoor + i, xCoor - 1] == 2)
                            map[yCoor + i, xCoor - 1] = 13;

                        if (map[yCoor + i, xCoor + 1] == 0 || map[yCoor + i, xCoor + 1] == 9 || map[yCoor + i, xCoor + 1] == 3)
                            map[yCoor + i, xCoor + 1] = 6;
                        else if (i == 0 && map[yCoor + i, xCoor + 1] == 8)
                            map[yCoor + i, xCoor + 1] = 10;
                        else if (map[yCoor + i, xCoor + 1] == 2)
                            map[yCoor + i, xCoor + 1] = 12;

                        if (((i + 1) < length) && (map[yCoor + i + 1, xCoor] == 5))
                            i = length;

                        break;
                    case 3:
                        map[yCoor, xCoor - i] = 5;
                        cMap[yCoor, xCoor - i] = true;

                        if (map[yCoor - 1, xCoor - i] == 0 || map[yCoor - 1, xCoor - i] == 1 || map[yCoor - 1, xCoor - i] == 3)
                            map[yCoor - 1, xCoor - i] = 2;
                        else if(i == 0 && map[yCoor - 1, xCoor - i] == 4)
                            map[yCoor - 1, xCoor - i] = 13;
                        else if (map[yCoor - 1, xCoor - i] == 6)
                            map[yCoor - 1, xCoor - i] = 12;

                        if (map[yCoor + 1, xCoor - i] == 0 || map[yCoor + 1, xCoor - i] == 7 || map[yCoor + 1, xCoor - i] == 9)
                            map[yCoor + 1, xCoor - i] = 8;
                        else if (i == 0 && map[yCoor + 1, xCoor - i] == 4)
                            map[yCoor + 1, xCoor - i] = 11;
                        else if (map[yCoor + 1, xCoor - i] == 6)
                            map[yCoor + 1, xCoor - i] = 10;

                        if (((i + 1) < length) && (map[yCoor, xCoor - i - 1] == 5))
                            i = length;

                        break;
                }
            }
            
        }

        //Places a room in the given tilemap using given coordinates and given constraints
        //enumerated at the top of the class.
        //Returns a Room object, which contains x,y coordinates, width, and height.
        public Room placeRoom(int[,] map, bool[,] cMap, int xCoor, int yCoor)
        {
            Room placedRoom;

            int randomWidthMax = random.Next(minRoomWidth, maxRoomWidth);
            int randomHeightMax = random.Next(minRoomHeight, maxRoomHeight);

            //Console.WriteLine("Random Width Max" + randomWidthMax);
            //Console.WriteLine("Random Height Max" + randomHeightMax);

            int leftMapBound = 0;
            int rightMapBound = map.GetLength(1) - 1;
            int upperMapBound = 0;
            int lowerMapBound = map.GetLength(0) - 1;
            int leftBound = xCoor - minRoomWidth;
            int rightBound = xCoor + minRoomWidth;
            int upperBound = yCoor - minRoomHeight;
            int lowerBound = yCoor + minRoomHeight;
            bool leftSet = false;
            bool rightSet = false;
            bool upperSet = false;
            bool lowerSet = false;

            //Console.WriteLine("X:" + xCoor);
            //Console.WriteLine("Y:" + yCoor);
            //Console.WriteLine("LeftBound:" + leftBound);
            //Console.WriteLine("Right Bound:" + rightBound);
            //Console.WriteLine("UpperBound:" + upperBound);
            //Console.WriteLine("Lower Bound:" + lowerBound);

            //for (int i = minRoomWidth; i < randomWidthMax; i++)
            //{
            //    if (map[yCoor, xCoor - i] != 0 || map[yCoor, xCoor + i] != 0)
            //    {
            //        leftBound = xCoor - i + 1;
            //        rightBound = xCoor + i - 1;
            //        i = randomWidthMax;
            //    }
            //}

            //for (int i = minRoomHeight; i < randomHeightMax; i++)
            //{
            //    if (map[yCoor - i, xCoor] != 0 || map[yCoor + i, xCoor] != 0)
            //    {
            //        upperBound = yCoor - i + 1;
            //        lowerBound = yCoor + i - 1;
            //        i = randomHeightMax;
            //    }
            //}

            int sizeCounter = (int)MathHelper.Min(minRoomHeight, minRoomWidth);
            while (sizeCounter <= randomWidthMax || sizeCounter <= randomHeightMax )
            {

                //Console.WriteLine(sizeCounter);

                if ((!leftSet || !upperSet) && map[yCoor - sizeCounter, xCoor - sizeCounter] != 0)
                {
                    //Console.WriteLine("Upper Left collision detected");
                    if (!leftSet)
                    {
                        leftBound = xCoor - sizeCounter + 1;
                        leftSet = true;
                    }
                    else if (!upperSet)
                    {
                        upperBound = yCoor - sizeCounter + 1;
                        upperSet = true;
                    }
                }

                if ((!rightSet || !upperSet) && map[yCoor - sizeCounter, xCoor + sizeCounter] != 0)
                {
                    //Console.WriteLine("Upper right collision detected");
                    if (!rightSet)
                    {
                        rightBound = xCoor + sizeCounter - 1;
                        rightSet = true;
                    }
                    if (!upperSet)
                    {
                        upperBound = yCoor - sizeCounter + 1;
                        upperSet = true;
                    }
                }

                if ((!leftSet || !lowerSet) && map[yCoor + sizeCounter, xCoor - sizeCounter] != 0)
                {
                    //Console.WriteLine("Lower Left collision detected");

                    if (!leftSet)
                    {
                        leftBound = xCoor - sizeCounter + 1;
                        leftSet = true;
                    }
                    if (!lowerSet)
                    {
                        lowerBound = yCoor + sizeCounter - 1;
                        lowerSet = true;
                    }
                }

                if ((!rightSet && !lowerSet) && map[yCoor + sizeCounter, xCoor + sizeCounter] != 0)
                {

                    //Console.WriteLine("Lower right collision detected");

                    if (!lowerSet)
                    {
                        lowerBound = yCoor + sizeCounter - 1;
                        lowerSet = true;
                    }
                    if (!rightSet)
                    {
                        rightBound = xCoor + sizeCounter - 1;
                        rightSet = true;
                    }
                }

                if (sizeCounter <= randomWidthMax)
                {
                    if (!leftSet)
                    {
                        if (map[yCoor, xCoor - sizeCounter] != 0)
                        {
                            leftBound = xCoor - sizeCounter + 1;
                            leftSet = true;
                        }
                        if (map[yCoor, xCoor - sizeCounter] == 0)
                        {
                            leftBound = xCoor - sizeCounter;
                        }
                    }

                    if (!rightSet)
                    {
                        if (map[yCoor, xCoor + sizeCounter] != 0)
                        {
                            rightBound = xCoor + sizeCounter - 1;
                            rightSet = true;
                        }
                        if (map[yCoor, xCoor + sizeCounter] == 0)
                        {
                            rightBound = xCoor + sizeCounter;
                        }
                    }
                }

                if (sizeCounter <= randomHeightMax)
                {
                    if (!upperSet)
                    {
                        if (map[yCoor - sizeCounter, xCoor] != 0)
                        {
                            upperBound = yCoor - sizeCounter + 1;
                            upperSet = true;
                        }
                        else if (map[yCoor - sizeCounter, xCoor] == 0)
                        {
                            upperBound = yCoor - sizeCounter;
                        }
                    }

                    if (!lowerSet)
                    {
                        if (map[yCoor + sizeCounter, xCoor] != 0)
                        {
                            lowerBound = yCoor + sizeCounter - 1;
                            lowerSet = true;
                        }
                        else if (map[yCoor + sizeCounter, xCoor] == 0)
                        {
                            lowerBound = yCoor + sizeCounter;
                        }

                    }
                }

                if (leftSet && rightSet && upperSet && lowerSet)
                {
                    //Console.WriteLine("Terminating loop - all bounds set");
                    sizeCounter = (int)MathHelper.Max(maxRoomWidth, maxRoomHeight) + 1;
                }

                sizeCounter++;

                if (xCoor - sizeCounter < leftMapBound || xCoor + sizeCounter > rightMapBound || yCoor - sizeCounter < upperMapBound || yCoor + sizeCounter > lowerMapBound)
                {
                    //Console.WriteLine("Terminating loop - room bounds went are illegal");
                    sizeCounter = (int)MathHelper.Max(maxRoomWidth, maxRoomHeight);
                }
            }

            //Console.WriteLine("LeftBound:" + leftBound);
            //Console.WriteLine("Right Bound:" + rightBound);
            //Console.WriteLine("UpperBound:" + upperBound);
            //Console.WriteLine("Lower Bound:" + lowerBound);

            map[upperBound, leftBound] = 1;
            map[upperBound, rightBound] = 3;
            map[lowerBound, leftBound] = 7;
            map[lowerBound, rightBound] = 9;

            for (int i = upperBound +1; i < lowerBound; i++)
            {
                map[i, leftBound] = 4;
                map[i, rightBound] = 6;
            }
            for (int i = leftBound +1; i < rightBound; i++)
            {
                map[upperBound, i] = 2;
                map[lowerBound, i] = 8;
            }

            for (int i = upperBound + 1; i < lowerBound; i++)
            {
                for (int j = leftBound + 1; j < rightBound; j++)
                {
                    map[i,j] = 5;
                    cMap[i, j] = true;
                }
            }
            //tileMap[yCoor, xCoor] = 14;

            //Console.WriteLine("Actual Width:" + (rightBound - leftBound + 1));
            //Console.WriteLine("Actual Height:" + (lowerBound - upperBound + 1));
            Rectangle dimensions = new Rectangle(leftBound, upperBound, rightBound - leftBound + 1, lowerBound - upperBound + 1);
            placedRoom = new Room(dimensions);
            return placedRoom;

        }
    }
}
