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
    [SerializeField] [Range(0, 100)] public int density;

    public int[,] allTiles;

    // Start is called before the first frame update
    void Start()
    {
        seed = Random.Range(100, 999);
        Random.InitState(seed);

        InitialiseTiles();
        DisplayMap();
    }

    private void InitialiseTiles()
    {
        tilemap.ClearAllTiles();

        allTiles = new int[size, size];

        for (int tileRow = 0; tileRow < size; tileRow++)
        {
            for (int tileCol = 0; tileCol < size; tileCol++)
            {
                allTiles[tileRow, tileCol] = Random.Range(0, 100);
            }
        }
    }

    private void DisplayMap()
    {
        for (int tileX = 0; tileX < size; tileX++)
        {
            for (int tileY = 0; tileY < size; tileY++)
            {
                if (allTiles[tileX, tileY] <= density)
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