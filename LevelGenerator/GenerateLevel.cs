using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Random = System.Random;

public class GenerateLevel : MonoBehaviour
{
    enum CellType {
        Room,
        Hallway,
        Empty
    }

    class Room {
        public BoundsInt bounds;

        public Room(Vector3Int location, Vector3Int size) {
            bounds = new BoundsInt(location, size);
        }

        public bool intersects(Room other) {
            return !((this.bounds.position.x <= other.bounds.position.x + other.bounds.size.x) && (this.bounds.position.x + this.bounds.size.x >= other.bounds.position.x) &&
                     (this.bounds.position.y <= other.bounds.position.y + other.bounds.size.y) && (this.bounds.position.y + this.bounds.size.y >= other.bounds.position.y) &&
                     (this.bounds.position.z <= other.bounds.position.z + other.bounds.size.z) && (this.bounds.position.z + this.bounds.size.z >= other.bounds.position.z));
        }
    }

    [SerializeField]
    Vector3Int levelSize;
    [SerializeField]
    int roomCount;
    [SerializeField]
    Vector3Int roomMaxSize;
    [SerializeField]
    GameObject cubePrefab;
    [SerializeField]
    Material redMaterial;
    [SerializeField]
    Material blueMaterial;

    Random random;
    List<Room> rooms;

    void Start()
    {
        random = new Random();
        rooms = new List<Room>();

        generateRooms();
    }

    void generateRooms()
    {
        for (int i = 0; i < roomCount; i++)
        {
            Vector3Int location = new Vector3Int(
                random.Next(0, levelSize.x),
                random.Next(0, levelSize.y),
                random.Next(0, levelSize.z)
            );

            Vector3Int roomSize = new Vector3Int(
                random.Next(1, roomMaxSize.x + 1),
                random.Next(1, roomMaxSize.y + 1),
                random.Next(1, roomMaxSize.z + 1)
            );

            bool canPutRoom = true;
        }

    }
}
