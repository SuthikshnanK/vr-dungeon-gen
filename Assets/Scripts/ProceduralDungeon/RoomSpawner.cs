using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RoomSpawner : MonoBehaviour
{
    /*
     *  OpeningDir : Indicates Direction of Next Node 
     *  
     *  1 : Top
     *  2 : Bottom
     *  3 : Left
     *  4 : Right
    */
    public int openingDir;

    private RoomTemplates templates;
    private int randRoom;
    public bool roomSpawned;

    private void Start()
    {
        templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
        Invoke(nameof(Spawn), 0.2f);
    }

    void Spawn()
    {
        if (!roomSpawned)
        {
            switch (openingDir)
            {
                case 1:
                    // Top Door
                    randRoom = Random.Range(0, templates.topRooms.Length);
                    Instantiate(templates.topRooms[randRoom], transform.position, templates.topRooms[randRoom].transform.rotation);
                    break;
                case 2:
                    // Bottom Door
                    randRoom = Random.Range(0, templates.bottomRooms.Length);
                    Instantiate(templates.bottomRooms[randRoom], transform.position, templates.bottomRooms[randRoom].transform.rotation);
                    break;
                case 3:
                    // Left Door
                    randRoom = Random.Range(0, templates.leftRooms.Length);
                    Instantiate(templates.leftRooms[randRoom], transform.position, templates.leftRooms[randRoom].transform.rotation);
                    break;
                case 4:
                    // Right Door
                    randRoom = Random.Range(0, templates.rightRooms.Length);
                    Instantiate(templates.rightRooms[randRoom], transform.position, templates.rightRooms[randRoom].transform.rotation);
                    break;
                default:
                    // This should only be true for the intial spawn point
                    break;
            }
            roomSpawned = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {

        if (!other.CompareTag("SpawnPoint"))
        {
            Destroy(gameObject);
        }
        if (other.GetComponent<RoomSpawner>() != null && other.CompareTag("SpawnPoint"))
        {
            if(other.GetComponent<RoomSpawner>().openingDir > openingDir)
            {
                Destroy(other.GetComponent<RoomSpawner>());
            }
        }
    }
}
