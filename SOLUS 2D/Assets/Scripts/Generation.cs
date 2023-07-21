using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class Generation : NetworkBehaviour
{
    public Tilemap tilemap;
    public Tile mainTile;

    private GameObject AudioManager;
    private AudioManager AudioManagerScript;

    [Range(0, 100)] public int density;
    [Range(1, 20)] public int iterations;
    public int MapSize;

    private int[,] allTiles;
    private int[,] updatedTiles;

    private int neighbourCount;

    private GameObject SeedBox;
    private TMP_Text SeedBoxText;

    public bool MapLoaded;

    public void Start()
    {
        AudioManager = GameObject.FindGameObjectWithTag("AudioManager");
        AudioManagerScript = AudioManager.GetComponent<AudioManager>();
    }

    public void LoadGame()
    {
        SceneManager.UnloadSceneAsync("MainMenu");

        var lastSceneIndex = SceneManager.sceneCount - 1;
        var lastLoadedScene = SceneManager.GetSceneAt(lastSceneIndex);
        SceneManager.UnloadSceneAsync(lastLoadedScene);
    }

    public void BeginGeneration(int seed)
    {
        MapLoaded = false;

        SeedBox = GameObject.FindGameObjectWithTag("SeedBox");
        SeedBoxText = SeedBox.GetComponent<TMP_Text>();
        SeedBoxText.text = ("Seed: " + seed);

        Random.InitState(seed);

        InitialiseTiles();

        for (int i = 0; i < iterations; i++)
        {
            Excavate();
        }

        DisplayMap();
        MapLoaded = true;
    }

    private void InitialiseTiles()
    {
        tilemap.ClearAllTiles();

        allTiles = new int[MapSize, MapSize];

        for (int tileX = 0; tileX < MapSize; tileX++)
        {
            for (int tileY = 0; tileY < MapSize; tileY++)
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

    private void Excavate()
    {
        updatedTiles = new int[MapSize, MapSize];
        for (int tileX = 1; tileX < MapSize - 1; tileX++)
        {
            for (int tileY = 1; tileY < MapSize - 1; tileY++)
            {
                neighbourCount = allTiles[tileX + 1, tileY + 0] + //RIGHT
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

    private void DisplayMap()
    {
        for (int tileX = 0; tileX < MapSize; tileX++)
        {
            for (int tileY = 0; tileY < MapSize; tileY++)
            {
                if (allTiles[tileX, tileY] == 0)
                {
                    tilemap.SetTile(new Vector3Int(tileX, tileY, 0), mainTile);
                }
            }
        }
    }

    public void ClearTiles()
    {
        tilemap.ClearAllTiles();
    }

    [ClientRpc]
    public void ClearTileClientRpc(Vector3Int Position, bool first)
    {
        tilemap.SetTile(Position, null);
        if (first)
        {
            AudioManagerScript.Play("Explode", Position);
        }
    }
}