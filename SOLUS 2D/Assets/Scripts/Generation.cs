using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Generation : MonoBehaviour
{
    public Tilemap tilemap;
    public Tile mainTile;

    public int size;
    public int seed;

    public int[,] allTiles;

    // Start is called before the first frame update
    void Start()
    {
        InitialiseTiles();
    }

    private void InitialiseTiles()
    {
        seed = Random.Range(100, 999);
        Random.InitState(seed);

        allTiles = new int[size * 2, size * 2];

        for (int tileRow = 0; tileRow < size * 2; tileRow++)
        {
            for (int tileCol = 0; tileCol < size * 2; tileCol++)
            {
                allTiles[tileRow, tileCol] = Random.Range(0, 2);
            }
        }

        for (int tileX = 0; tileX < size * 2; tileX++)
        {
            for (int tileY = 0; tileY < size * 2; tileY++)
            {
                if (allTiles[tileX, tileY] == 1)
                {
                    tilemap.SetTile(new Vector3Int(tileX, tileY, 0), mainTile);
                }
            }
        }

    }

    private void Excavate()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}