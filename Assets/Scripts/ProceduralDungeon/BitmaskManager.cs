using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BitmaskManager : MonoBehaviour
{
    public GameObject world;
    public GameObject[] debugMappings;
    public float spacing = 20f;

    public NavMeshSurface surface;

    public GameObject[] prefabBitmask;

    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public void Start()
    {

    }

    public int[,] CalculateBitmask(int[,] map)
    {
        int[,] bitMap = new int[map.GetLength(0), map.GetLength(1)];
        for(int x = 1; x < map.GetLength(0)-1; x++)
        {
            for(int y = 1; y < map.GetLength(1)-1; y++)
            {
                int[] cardinals = new int[4];
                if(map[x,y] != 0) // This means the current tile belongs to a Room
                {
                    bitMap[x, y] = 0;
                    if (map[x, y + 1] != 0) // Top
                    {
                        cardinals[0] = 1;
                    }
                    if (map[x - 1, y] != 0) // Left
                    {
                        cardinals[1] = 1;
                    }
                    if (map[x + 1, y] != 0) // Right
                    {
                        cardinals[2] = 1;
                    }
                    if (map[x, y - 1] != 0) // Bottom
                    {
                        cardinals[3] = 1;
                    }
                    for(int i = 0; i < 4; i++)
                    {
                        bitMap[x,y] += (int)Mathf.Pow(2, i) * cardinals[i];
                    }
                }
            }
        }
        return bitMap;
    }

    public void DrawDebug(int[,] map)
    {
        int[,] bitMap = CalculateBitmask(map);
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                switch (map[i, j])
                {
                    case 1:
                        GameObject roomTile = Instantiate(debugMappings[1], new Vector3(spacing * i, 1f, spacing * j), transform.rotation, world.transform);
                        roomTile.transform.localScale = new Vector3(1.8f, 1f, 1.8f);
                        roomTile.name = "RoomTile " + i + "-" + j + " Bitmask: " + bitMap[i,j];
                        break;
                    case 2:
                        GameObject corridorTile = Instantiate(debugMappings[2], new Vector3(spacing * i, 1f, spacing * j), transform.rotation, world.transform);
                        corridorTile.transform.localScale = new Vector3(1.8f, 1f, 1.8f);
                        corridorTile.name = "Corridor " + i + "-" + j + " Bitmask: " + bitMap[i,j];
                        break;
                    case 3:
                        GameObject spawnTile = Instantiate(debugMappings[3], new Vector3(spacing * i, 1f, spacing * j), transform.rotation, world.transform);
                        spawnTile.transform.localScale = new Vector3(1.8f, 1f, 1.8f);
                        spawnTile.name = "Spawn " + i + "-" + j;
                        break;
                    case 4:
                        GameObject enemyTile = Instantiate(debugMappings[4], new Vector3(spacing * i, 1f, spacing * j), transform.rotation, world.transform);
                        enemyTile.transform.localScale = new Vector3(1.8f, 1f, 1.8f);
                        enemyTile.name = "EnemySpawn " + i + "-" + j;
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public void DrawMap(int[,] map)
    {
        int[,] bitMap = CalculateBitmask(map);
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if(bitMap[i,j] != 0)
                {
                    GameObject currentTile = Instantiate(prefabBitmask[bitMap[i, j]], new Vector3(spacing * i, 0f, spacing * j), transform.rotation, world.transform);
                    currentTile.name += " RoomTile " + i + "-" + j + " Bitmask: " + bitMap[i, j];
                    if(map[i,j] == 3)
                    {
                        Instantiate(playerPrefab, new Vector3(spacing * i, 0f, spacing * j), playerPrefab.transform.rotation);
                    }
                    else if(map[i,j] == 4)
                    {
                        Instantiate(enemyPrefab, new Vector3(spacing * i, 0f, spacing * j), enemyPrefab.transform.rotation);
                    }
                }
            }
        }//
        surface.BuildNavMesh();
    }

    public void DrawCell(SubDungeon dungeon)
    {
        GameObject clone = Instantiate(debugMappings[2], new Vector3(spacing * (dungeon.x + dungeon.width / 2) - 10, -1f, spacing * (dungeon.y + dungeon.height / 2) - 10), transform.rotation, world.transform);
        clone.transform.localScale = new Vector3(dungeon.width * 1.8f, 1f, dungeon.height * 1.8f);
        clone.name = "Cell XY: " + dungeon.x + " " + dungeon.y + " Scale: " + dungeon.width + " " + dungeon.height;
    }
}
