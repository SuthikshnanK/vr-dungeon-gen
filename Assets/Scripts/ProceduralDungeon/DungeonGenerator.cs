using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    private BitmaskManager bitmask;
    public GameObject world;

    public bool renderDebug = true;

    public int width = 25;
    public int height = 25;

    public float spacing = 20f;

    public int roomDefaultSpawnPercent = 25;

    public int maxRoomWidth = 8;
    public int maxRoomHeight = 8;

    public Stack<SubDungeon> dStack;
    public Queue<SubDungeon> dQueue;

    public int[,] map;

    public int numEnemiesModifier = 3;
    public int maxEnemiesPerRoom = 5;

    // Start is called before the first frame update
    void Start()
    {
        bitmask = GetComponent<BitmaskManager>();
        map = new int[width, height];
        dStack = new Stack<SubDungeon>();
        dQueue = new Queue<SubDungeon>();
        Generate();
    }

    public void Generate()
    {
        SubDungeon root = new SubDungeon(1, 1, width - 2, height - 2);
        dQueue.Enqueue(root);

        while (dQueue.Count >= 1)
        {
            //print("Queue Length: "+dQueue.Count);
            SubDungeon level = dQueue.Dequeue();
            if(level.leftDungeon == null && level.rightDungeon == null)
            {
                if(level.width > maxRoomWidth || level.height > maxRoomHeight || Random.Range(0,100) > roomDefaultSpawnPercent)
                {
                    if (level.Divide())
                    {
                        dQueue.Enqueue(level.leftDungeon);
                        dQueue.Enqueue(level.rightDungeon);
                    }
                }
            }
        }

        root.CreateRooms();

        dStack.Clear();
        dStack.Push(root);
        while(dStack.Count >= 1)
        {
            SubDungeon currentNode = dStack.Pop();
            if(currentNode.leftDungeon != null || currentNode.rightDungeon != null)
            {
                if(currentNode.leftDungeon != null)
                {
                    dStack.Push(currentNode.leftDungeon);
                }
                if(currentNode.rightDungeon != null)
                {
                    dStack.Push(currentNode.rightDungeon);
                }

                if(currentNode.leftDungeon != null && currentNode.rightDungeon != null)
                {
                    while(currentNode.halls.Count >= 1)
                    {
                        AddRoom(currentNode.halls.Dequeue(),2);
                    }
                }
            }
            else
            {
                if(currentNode.room != null)
                {
                    // DrawCell(currentNode);
                    AddRoom(currentNode.room,1);
                }
            }
        }

        dStack.Clear();
        dStack.Push(root);
        while(dStack.Count >= 1)
        {
            SubDungeon currentNode = dStack.Pop();
            if(currentNode.leftDungeon != null)
            {
                dStack.Push(currentNode.leftDungeon);
            }
            else if (currentNode.leftDungeon == null && currentNode.room != null)
            {
                CreateSafeZone(currentNode.room);
                ChangeTile(currentNode.room.GetRoomCenter(), 3);
            }
        }

        TrimMapEdges(); // Cleans up the edges to prevent clipping issues

        if (!renderDebug)
        {
            bitmask.DrawMap(map);
        }
        else
        {
            bitmask.DrawDebug(map);
        }
    }

    public void TrimMapEdges()
    {
        for(int x = 0; x < map.GetLength(0); x++)
        {
            map[x, 0] = 0;
            map[x, map.GetLength(1) - 1] = 0;
        }
        for (int y = 0; y < map.GetLength(1); y++)
        {
            map[0, y] = 0;
            map[map.GetLength(0) - 1, y] = 0;
        }
    }

    public void AddRoom(Room room, int type)
    {
        int x = (int)room.position.x;
        int y = (int)room.position.y;
        int width = (int)room.dimensions.x;
        int height = (int)room.dimensions.y;

        int numEnemies = Random.Range(1, (width/numEnemiesModifier)*(height/numEnemiesModifier));
        if(numEnemies > maxEnemiesPerRoom)
        {
            numEnemies = maxEnemiesPerRoom;
        }

        if (x + width < map.GetLength(0) && y + height < map.GetLength(1))
        {
            for (int i = x; i < x + width; i++)
            {
                for (int j = y; j < y + height; j++)
                {
                    map[i, j] = type;
                }
            }

            int numTries = 3;
            while(numEnemies >= 0 && numTries >= 0)
            {
                int xCoord = Random.Range(x+1, x + width-1);
                int yCoord = Random.Range(y+1, y + height-1);
                if(map[xCoord,yCoord] == 1)
                {
                    map[xCoord, yCoord] = 4;
                    numEnemies--;
                    numTries = 3;
                }
                else
                {
                    numTries--;
                }
            }

        }
        else
        {
            Debug.LogError("Cannot Add Room: Room out of Bounds");
        }
    }

    public void CreateSafeZone(Room room)
    {
        int x = (int)room.position.x;
        int y = (int)room.position.y;
        int width = (int)room.dimensions.x;
        int height = (int)room.dimensions.y;

        if (x + width < map.GetLength(0) && y + height < map.GetLength(1))
        {
            for (int i = x; i < x + width; i++)
            {
                for (int j = y; j < y + height; j++)
                {
                    map[i, j] = 1;
                }
            }

        }
        else
        {
            Debug.LogError("Cannot Create Safe Zone: Room out of Bounds");
        }
    }

    public void ChangeTile(Vector2 tilePosition, int type)
    {
        map[(int)tilePosition.x, (int)tilePosition.y] = type;
    }
}

public class Room
{

    public Vector2 position;
    public Vector2 dimensions;

    public Room(Vector2 pos, Vector2 dims)
    {
        position = pos;
        dimensions = dims;
    }

    public Vector2 GetRoomCenter()
    {
        return new Vector2(position.x + (dimensions.x / 2), position.y + (dimensions.y / 2));
    }

}

public class SubDungeon
{
    [SerializeField]
    private int minRoomSize = 6;
    private int minCellSize = 7;

    public SubDungeon leftDungeon;
    public SubDungeon rightDungeon;

    public int x;
    public int y;
    public int width;
    public int height;

    public Room room;
    public Queue<Room> halls;
    
    public SubDungeon(int _x, int _y, int _width, int _height)
    {
        x = _x;
        y = _y;
        width = _width;
        height = _height;
        halls = new Queue<Room>();
    }

    public void CreateRooms()
    {
        if(leftDungeon != null || rightDungeon != null)
        {
            if(leftDungeon != null)
            {
                leftDungeon.CreateRooms();
            }
            if(rightDungeon != null)
            {
                rightDungeon.CreateRooms();
            }

            if(leftDungeon != null && rightDungeon != null)
            {
                CreateHall(leftDungeon.GetRoom(), rightDungeon.GetRoom());
            }
        }
        else
        {
            Vector2 roomSize = new Vector2(Random.Range(minRoomSize, width - 2), Random.Range(minRoomSize, height - 2));
            Vector2 roomPos = new Vector2(x + Random.Range(1, width - roomSize.x - 1), y + Random.Range(1, height - roomSize.y - 1));
            room = new Room(roomPos, roomSize);
            //Debug.Log("Room was Created");
        }
    }

    public Room GetRoom()
    {
        if(room != null)
        {
            return room;
        }
        else
        {
            Room lRoom = null;
            Room rRoom = null;

            if(leftDungeon != null)
            {
                lRoom = leftDungeon.GetRoom();
            }
            if(rightDungeon != null)
            {
                rRoom = rightDungeon.GetRoom();
            }
            if(lRoom == null && rRoom == null)
            {
                return null;
            }
            else if(rRoom == null)
            {
                return lRoom;
            }
            else if(lRoom == null)
            {
                return rRoom;
            }
            else if(Random.Range(0,1) > .5)
            {
                return lRoom;
            }
            else
            {
                return rRoom;
            }
        }
    }

    public void CreateHall(Room roomA, Room roomB)
    {
        Vector2 aPos = roomA.position;
        Vector2 aDim = roomA.dimensions;
        Vector2 aEdge = new Vector2(aPos.x + aDim.x, aPos.y + aDim.y);
        Vector2 bPos = roomB.position;
        Vector2 bDim = roomB.dimensions;
        Vector2 bEdge = new Vector2(bPos.x + bDim.x, bPos.y + bDim.y);

        // Creating Opposite Corners for ease of access
        Vector2 aCorn = new Vector2(Random.Range(aPos.x + 1, aEdge.x - 2), Random.Range(aPos.y + 1, aEdge.y - 2));
        Vector2 bCorn = new Vector2(Random.Range(bPos.x + 1, bEdge.x - 2), Random.Range(bPos.y + 1, bEdge.y - 2));

        float w = bCorn.x - aCorn.x;
        float h = bCorn.y - aCorn.y;
        if(w < 0)
        {
            if (h < 0)
            {
                if(Random.Range(0, 1) < 0.5)
                {
                    halls.Enqueue(new Room(new Vector2(bCorn.x, aCorn.y), new Vector2(Mathf.Abs(w), 1)));
                    halls.Enqueue(new Room(new Vector2(bCorn.x, bCorn.y), new Vector2(1, Mathf.Abs(h))));
                }
                else
                {
                    halls.Enqueue(new Room(new Vector2(bCorn.x, bCorn.y), new Vector2(Mathf.Abs(w), 1)));
                    halls.Enqueue(new Room(new Vector2(aCorn.x, bCorn.y), new Vector2(1, Mathf.Abs(h))));
                }
            }
            else if(h > 0)
            {
                if (Random.Range(0, 1) < 0.5)
                {
                    halls.Enqueue(new Room(new Vector2(bCorn.x, aCorn.y), new Vector2(Mathf.Abs(w), 1)));
                    halls.Enqueue(new Room(new Vector2(bCorn.x, aCorn.y), new Vector2(1, Mathf.Abs(h))));
                }
                else
                {
                    halls.Enqueue(new Room(new Vector2(bCorn.x, bCorn.y), new Vector2(Mathf.Abs(w), 1)));
                    halls.Enqueue(new Room(new Vector2(aCorn.x, aCorn.y), new Vector2(1, Mathf.Abs(h))));
                }
            }
            else
            {
                halls.Enqueue(new Room(new Vector2(bCorn.x, bCorn.y), new Vector2(Mathf.Abs(w), 1)));
            }
        }
        else if(w > 0)
        {
            if (h < 0)
            {
                if (Random.Range(0, 1) < 0.5)
                {
                    halls.Enqueue(new Room(new Vector2(aCorn.x, bCorn.y), new Vector2(Mathf.Abs(w), 1)));
                    halls.Enqueue(new Room(new Vector2(aCorn.x, bCorn.y), new Vector2(1, Mathf.Abs(h))));
                }
                else
                {
                    halls.Enqueue(new Room(new Vector2(aCorn.x, aCorn.y), new Vector2(Mathf.Abs(w), 1)));
                    halls.Enqueue(new Room(new Vector2(bCorn.x, bCorn.y), new Vector2(1, Mathf.Abs(h))));
                }
            }
            else if (h > 0)
            {
                if (Random.Range(0, 1) < 0.5)
                {
                    halls.Enqueue(new Room(new Vector2(aCorn.x, aCorn.y), new Vector2(Mathf.Abs(w), 1)));
                    halls.Enqueue(new Room(new Vector2(bCorn.x, aCorn.y), new Vector2(1, Mathf.Abs(h))));
                }
                else
                {
                    halls.Enqueue(new Room(new Vector2(aCorn.x, bCorn.y), new Vector2(Mathf.Abs(w), 1)));
                    halls.Enqueue(new Room(new Vector2(aCorn.x, aCorn.y), new Vector2(1, Mathf.Abs(h))));
                }
            }
            else
            {
                halls.Enqueue(new Room(new Vector2(aCorn.x, aCorn.y), new Vector2(Mathf.Abs(w), 1)));
            }

        }
        else if(w == 0)
        {
            if(h > 0)
            {
                halls.Enqueue(new Room(new Vector2(bCorn.x, bCorn.y), new Vector2(1, Mathf.Abs(h))));
            }
            else if(h > 0)
            {
                halls.Enqueue(new Room(new Vector2(aCorn.x, aCorn.y), new Vector2(1, Mathf.Abs(h))));
            }
        }
    }

    public bool Divide()
    {
        // Debug.Log("Attempting to Divide");
        if(leftDungeon != null && rightDungeon != null)
        {
            return false;
        }

        bool verticalSplit = Random.Range(0, 1) > 0.5;
        if(width > height && width / height > 1.25f)
        {
            verticalSplit = true;
        }
        else if(height > width && height / width > 1.25f)
        {
            verticalSplit = false;
        }

        int maxSplitSize = (verticalSplit ? width : height) - minCellSize;
        if(maxSplitSize <= minCellSize)
        {
            return false;
        }

        int splitPosition = Random.Range(minRoomSize, maxSplitSize);
        if (verticalSplit)
        {
            leftDungeon = new SubDungeon(x, y, splitPosition, height);
            rightDungeon = new SubDungeon(x + splitPosition, y, width - splitPosition, height);
        }
        else
        {
            leftDungeon = new SubDungeon(x, y, width, splitPosition);
            rightDungeon = new SubDungeon(x, y + splitPosition, width, height - splitPosition);
        }
        // Debug.Log("Division was Successful!");
        return true;
    }
}
