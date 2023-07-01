using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.Netcode;

public class Generation : NetworkBehaviour
{
    public Tilemap tilemap;
    public Tile mainTile;
    public Tile powerTile;
    public Tile ammoTile;

    public int size;
    private int seed;
    [SerializeField] [Range(0, 100)] public int density;
    [SerializeField] [Range(1, 20)] public int iterations;

    private int[,] allTiles;
    private int[,] updatedTiles;

    private int neighbourCount;

    // Start is called before the first frame update
    void Start()
    {
        if (IsClient) return;
        seed = Random.Range(1000000, 9999999); //8024669
        Debug.Log("Seed: " + seed);

        Random.InitState(seed);

        InitialiseTiles();

        for (int i = 0; i < iterations; i++)
        {
            Excavate();
        }

        DisplayMap();
    }

    private void InitialiseTiles()
    {
        tilemap.ClearAllTiles();

        allTiles = new int[size, size];

        for (int tileX = 0; tileX < size; tileX++)
        {
            for (int tileY = 0; tileY < size; tileY++)
            {
                allTiles[tileX, tileY] = Random.Range(0, 100);
                if (allTiles[tileX, tileY] <= density)
                {
                    allTiles[tileX, tileY] = 1;
                }

                else
                {
                    allTiles[tileX, tileY] = 0;
                }
            }
        }
    }

    private void DisplayMap()
    {
        for (int tileX = 0; tileX < size; tileX++)
        {
            for (int tileY = 0; tileY < size; tileY++)
            {
                if (allTiles[tileX, tileY] == 0)
                {
                    tilemap.SetTile(new Vector3Int(tileX, tileY, 0), mainTile);
                }
            }
        }
    }

    private void Excavate()
    {
        updatedTiles = new int[size, size];
        for (int tileX = 1; tileX < size - 1; tileX++)
        {
            for (int tileY = 1; tileY < size - 1; tileY++)
            {
                neighbourCount =    allTiles[tileX + 1, tileY + 0] + //RIGHT
                                    allTiles[tileX + 1, tileY + 1] + //BOTTOM RIGHT
                                    allTiles[tileX + 0, tileY + 1] + //BOTTOM
                                    allTiles[tileX - 1, tileY + 1] + //BOTTOM LEFT
                                    allTiles[tileX - 1, tileY + 0] + //LEFT
                                    allTiles[tileX - 1, tileY - 1] + //TOP LEFT
                                    allTiles[tileX + 0, tileY - 1] + //TOP
                                    allTiles[tileX + 1, tileY - 1]; //TOP RIGHT

                if (neighbourCount > 4)
                {
                    updatedTiles[tileX, tileY] = 1;
                }

                else
                {
                    updatedTiles[tileX, tileY] = 0;
                }
            }
        }

        allTiles = updatedTiles;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}