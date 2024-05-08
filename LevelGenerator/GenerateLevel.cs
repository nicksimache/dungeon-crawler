using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;
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
            return (this.bounds.position.x <= other.bounds.position.x + other.bounds.size.x) && (this.bounds.position.x + this.bounds.size.x >= other.bounds.position.x) &&
                   (this.bounds.position.y <= other.bounds.position.y + other.bounds.size.y) && (this.bounds.position.y + this.bounds.size.y >= other.bounds.position.y) &&
                   (this.bounds.position.z <= other.bounds.position.z + other.bounds.size.z) && (this.bounds.position.z + this.bounds.size.z >= other.bounds.position.z);
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
    Grid3D<CellType> grid;
    List<Room> rooms;

    void Start()
    {
        random = new Random();
        grid = new Grid3D<CellType>(levelSize, Vector3Int.zero);
        rooms = new List<Room>();

        generateRooms();
    }

    void generateRooms()
    {
        for (int i = 0; i < roomCount; i++)
        {
            Vector3Int location = new Vector3Int(
                random.Next(0, levelSize.x),
                0,
                random.Next(0, levelSize.z)
            );

            Vector3Int roomSize = new Vector3Int(
                random.Next(1, roomMaxSize.x + 1),
                random.Next(1, roomMaxSize.y + 1),
                random.Next(1, roomMaxSize.z + 1)
            );

            bool canPlaceRoom = true;
            Room newRoom = new Room(location, roomSize);
            Room buffer = new Room(location + new Vector3Int(-1, 0, -1), roomSize + new Vector3Int(2, 0, 2));

            foreach (var room in rooms)
            {
                if (room.intersects(buffer))
                {
                    canPlaceRoom = false;
                    break;
                }
            }

            if (newRoom.bounds.xMin < 0 || newRoom.bounds.xMax >= levelSize.x
                || newRoom.bounds.yMin < 0 || newRoom.bounds.yMax >= levelSize.y
                || newRoom.bounds.zMin < 0 || newRoom.bounds.zMax >= levelSize.z)
            {
                canPlaceRoom = false;
            }

            if (canPlaceRoom)
            {
                rooms.Add(newRoom);
                PlaceRoom(newRoom.bounds.position, newRoom.bounds.size);

                foreach (var pos in newRoom.bounds.allPositionsWithin)
                {
                    grid[pos] = CellType.Room;
                }
            }
        }

        void PlaceCube(Vector3Int location, Vector3Int size, Material material)
        {
            GameObject go = Instantiate(cubePrefab, location, Quaternion.identity);
            go.GetComponent<Transform>().localScale = size;
            go.GetComponent<MeshRenderer>().material = material;
        }

        void PlaceRoom(Vector3Int location, Vector3Int size)
        {
            PlaceCube(location, size, redMaterial);
        }


    }
}
