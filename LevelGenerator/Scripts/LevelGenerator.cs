using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
using Graphs;
using UnityEngine.UIElements;
using UnityEngine.UI;


public class Generator2D : MonoBehaviour
{
    enum CellType
    {
        None,
        Room,
        MainHallway,
        Hallway,
        LockedDoor,
        Door,
        BossRoom
    }

    class Room
    {
        public RectInt bounds;
        public bool bossRoom;

        public Room(Vector2Int location, Vector2Int size)
        {
            bounds = new RectInt(location, size);
            bossRoom = false;
        }

        public static bool Intersect(Room a, Room b)
        {
            return !((a.bounds.position.x >= (b.bounds.position.x + b.bounds.size.x)) || ((a.bounds.position.x + a.bounds.size.x) <= b.bounds.position.x)
                || (a.bounds.position.y >= (b.bounds.position.y + b.bounds.size.y)) || ((a.bounds.position.y + a.bounds.size.y) <= b.bounds.position.y));
        }

    }

    public static class Vector2IntExtensions
    {
        public static readonly Vector2Int up = new Vector2Int(0, 1);
        public static readonly Vector2Int down = new Vector2Int(0, -1);
        public static readonly Vector2Int right = new Vector2Int(1, 0);
        public static readonly Vector2Int left = new Vector2Int(-1, 0);
    }

    [SerializeField]
    Vector2Int size;
    [SerializeField]
    int mandatoryRooms;
    [SerializeField]
    int optionalRooms;
    [SerializeField]
    Vector2Int roomMaxSize;
    [SerializeField]
    GameObject cubePrefab;
    [SerializeField]
    Material redMaterial;
    [SerializeField]
    Material blueMaterial;
    [SerializeField]
    Material purpleMaterial;
    [SerializeField]
    Material greenMaterial;
    [SerializeField]
    Material orangeMaterial;
    [SerializeField]
    Material blackMaterial;

    Random random;
    Grid2D<CellType> grid;
    List<Room> rooms;
    Delaunay delaunay;
    HashSet<Prim.Edge> selectedEdges;

    void Start()
    {
        Generate();
    }

    void Generate()
    {
        random = new Random();
        grid = new Grid2D<CellType>(size + new Vector2Int(20,20), Vector2Int.zero);
        rooms = new List<Room>();

        PlaceRooms(mandatoryRooms, true);
        Triangulate();
        CreateHallways(true);
        PathfindHallways(true);

        PlaceRooms(optionalRooms, false);
        Triangulate();
        CreateHallways(false);
        PathfindHallways(false);
    }

    void PlaceRooms(int roomCount, bool createMainRooms)
    {
        while(roomCount > 0)
        {
            Vector2Int location = new Vector2Int(
                random.Next(0, size.x),
                random.Next(0, size.y)
            );

            Vector2Int roomSize = new Vector2Int(
                2*random.Next(1, roomMaxSize.x+1)+1,
                2*random.Next(1, roomMaxSize.y+1)+1
            );

            bool add = true;
            Room newRoom = new Room(location, roomSize);
            Room buffer = new Room(location + new Vector2Int(-1, -1), roomSize + new Vector2Int(2, 2));

            foreach (var room in rooms)
            {
                if (Room.Intersect(room, buffer))
                {
                    add = false;
                    break;
                }
            }

            if (newRoom.bounds.xMin < 0 || newRoom.bounds.xMax >= size.x
                || newRoom.bounds.yMin < 0 || newRoom.bounds.yMax >= size.y)
            {
                add = false;
            }

            foreach (var pos in newRoom.bounds.allPositionsWithin)
            {
                if (grid[pos] == CellType.MainHallway)
                {
                    add = false;
                    break;
                }
            }

            if (add)
            {
                rooms.Add(newRoom);

                if (roomCount == 1 && createMainRooms)
                {
                    newRoom.bossRoom = true;
                    PlaceBossRoom(newRoom.bounds.position, newRoom.bounds.size);
                }
                else
                {
                    PlaceRoom(newRoom.bounds.position, newRoom.bounds.size);
                }
                
                foreach (var pos in newRoom.bounds.allPositionsWithin)
                {
                    grid[pos] = CellType.Room;
                }
                roomCount--;
            }
        }
    }

    void Triangulate()
    {
        List<Graphs.Vertex> vertices = new List<Graphs.Vertex>();

        foreach (var room in rooms)
        {
            vertices.Add(new Vertex<Room>((Vector2)room.bounds.position + ((Vector2)room.bounds.size) / 2, room));
        }

        delaunay = Delaunay.Triangulate(vertices);
    }

    void CreateHallways(bool createMainHallways)
    {
        List<Prim.Edge> edges = new List<Prim.Edge>();

        foreach (var edge in delaunay.Edges)
        {
            edges.Add(new Prim.Edge(edge.U, edge.V));
        }

        List<Prim.Edge> mst = Prim.MinimumSpanningTree(edges, edges[0].U);

        selectedEdges = new HashSet<Prim.Edge>(mst);
        var remainingEdges = new HashSet<Prim.Edge>(edges);
        remainingEdges.ExceptWith(selectedEdges);

        foreach (var edge in remainingEdges)
        {
            if (random.NextDouble() < 0.125)
            {
                selectedEdges.Add(edge);
            }
        }
    }

    void PathfindHallways(bool createMainHallways)
    {
        DungeonPathfinder aStar = new DungeonPathfinder(size);

        foreach (var edge in selectedEdges)
        {
            var startRoom = (edge.U as Vertex<Room>).Item;
            var endRoom = (edge.V as Vertex<Room>).Item;

            var startPosf = startRoom.bounds.center;
            var endPosf = endRoom.bounds.center;
            var startPos = new Vector2Int((int)startPosf.x, (int)startPosf.y);
            var endPos = new Vector2Int((int)endPosf.x, (int)endPosf.y);

            var path = aStar.FindPath(startPos, endPos, (DungeonPathfinder.Node a, DungeonPathfinder.Node b) => {
                var pathCost = new DungeonPathfinder.PathCost();

                pathCost.cost = Vector2Int.Distance(b.Position, endPos);    //heuristic

                if (grid[b.Position] == CellType.Room)
                {
                    pathCost.cost += 10;
                }
                else if (grid[b.Position] == CellType.None)
                {
                    pathCost.cost += 5;
                }
                else if (grid[b.Position] == CellType.Hallway || grid[b.Position] == CellType.MainHallway)
                {
                    pathCost.cost += 1;
                }

                pathCost.traversable = true;

                return pathCost;
            });

            if (path != null)
            {

                if(createMainHallways)
                {

                    bool madeDoorRoom = false;
                    bool madeDoorRoomEnd = false;

                    for(int i = 0; i < path.Count; i++)
                    {
                        var current = path[i];

                        if (grid[current] == CellType.None)
                        {
                            if(!madeDoorRoom && (grid[current] == CellType.Door || grid[current] == CellType.LockedDoor))
                            {
                                madeDoorRoom = true;
                            }
                            else if (!madeDoorRoom)
                            {
                                madeDoorRoom = true;
                                grid[current] = CellType.Door;
                            }
                            else if (!madeDoorRoomEnd && grid[path[i + 1]] == CellType.Room)
                            {
                                madeDoorRoomEnd = true;
                                grid[current] = CellType.Door;

                            }
                            else
                            {
                                grid[current] = CellType.MainHallway;
                            }
                        }

                        
                    }
                }
                else
                {
                    bool madeLockedRoom = false;
                    bool madeDoorRoomEnd = false;

                    for (int i = 0; i < path.Count; i++)
                    {
                        var current = path[i];

                        if (grid[current] == CellType.None)
                        {
                            if (!madeLockedRoom && (grid[current] == CellType.Door || grid[current] == CellType.LockedDoor))
                            {
                                madeLockedRoom = true;
                            }
                            else if (!madeLockedRoom)
                            {
                                madeLockedRoom = true;
                                grid[current] = CellType.LockedDoor;
                            }
                            else if (!madeDoorRoomEnd && grid[path[i + 1]] == CellType.Room)
                            {
                                madeDoorRoomEnd = true;
                                grid[current] = CellType.Door;

                            }
                            else
                            {
                                grid[current] = CellType.Hallway;
                            }
                        }


                    }
                }

                foreach (var pos in path)
                {
                    if (grid[pos] == CellType.Hallway)
                    {
                        PlaceHallway(pos);
                    }
                    else if (grid[pos] == CellType.MainHallway)
                    {
                        PlaceMainHallway(pos);
                    }
                    else if (grid[pos] == CellType.LockedDoor)
                    {
                        PlaceLockedRoom(pos);
                    }
                    else if (grid[pos] == CellType.Door)
                    {
                        PlaceDoorRoom(pos);
                    }

                }
            }
        }
    }

    void PlaceRoom(Vector2Int location, Vector2Int size)
    {
        var positions = GetRoomPositions(location, size);
        PlaceCube(positions[positions.Count / 2], size, redMaterial);
    }

    void PlaceBossRoom(Vector2Int location, Vector2Int size)
    {
        var positions = GetRoomPositions(location, size);
        PlaceCube(positions[positions.Count / 2], size, blackMaterial);
    }
    void PlaceHallway(Vector2Int location)
    {
        
        PlaceCube(location, new Vector2Int(1, 1), blueMaterial);

    }

    void PlaceLockedRoom(Vector2Int location)
    {
        PlaceCube(location, new Vector2Int(1, 1), orangeMaterial);
    }

    void PlaceDoorRoom(Vector2Int location)
    {
        PlaceCube(location, new Vector2Int(1, 1), greenMaterial);
    }

    void PlaceMainHallway(Vector2Int location)
    {
        PlaceCube(location, new Vector2Int(1,1), purpleMaterial);
    }

    void PlaceCube(Vector2Int location, Vector2Int size, Material material)
    {
        GameObject go = Instantiate(cubePrefab, new Vector3(location.x, 0, location.y), Quaternion.identity);
        go.GetComponent<Transform>().localScale = new Vector3(size.x, 1, size.y);
        go.GetComponent<MeshRenderer>().material = material;
    }

    List<Vector2Int> GetRoomPositions(Vector2Int location, Vector2Int size)
    {
        List<Vector2Int> positions = new List<Vector2Int>();

        for (int x = location.x; x < location.x + size.x; x++)
        {
            for (int y = location.y; y < location.y + size.y; y++)
            {
                positions.Add(new Vector2Int(x, y));
            }
        }

        return positions;
    }
}