using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Random = System.Random;


public class Begin : MonoBehaviour
{

    enum type
    {
        empty,
        block
    }

    Random random;
    Grid2D<type> grid;

    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Material red;

    void placeObstacle()
    {
        random = new Random();
        grid = new Grid2D<type>(new Vector2Int(1000, 1000), new Vector2Int(500,500));


        for (int i = 0; i < 30; i++)
        {

            Vector2Int location = new Vector2Int(
                random.Next(-50, 50),
                random.Next(-50, 50)
            );

            Vector2Int size = new Vector2Int(
                random.Next(5, 6),
                random.Next(5, 6)

            );

            //PlaceCube(location, size, red);

            RectInt bounds = new RectInt(location, size);

            foreach(var pos in bounds.allPositionsWithin)
            {
                grid[pos] = type.block;
            }
        }

    }

    void Start()
    {
        placeObstacle();
        
    }

    private void Update()
    {
        if (Input.GetKeyDown("x"))
        {
            Instantiate(enemyPrefab, new Vector3Int(random.Next(-50, 50), 0, random.Next(-50, 50)), Quaternion.identity);
        }
    }

    void PlaceCube(Vector2Int location, Vector2Int size, Material material)
    {
        GameObject go = Instantiate(cubePrefab, new Vector3(location.x, -1, location.y), Quaternion.identity);
        go.GetComponent<Transform>().localScale = new Vector3(size.x, 6, size.y);
        go.GetComponent<MeshRenderer>().material = material;

    }
}
