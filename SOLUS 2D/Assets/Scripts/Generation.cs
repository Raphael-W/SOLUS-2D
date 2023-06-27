using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Generation : MonoBehaviour
{
    public Tilemap tilemap;
    public Tile mainTile;
    public Vector3Int tilePos;

    // Start is called before the first frame update
    void Start()
    {
        tilePos = new Vector3Int(0, 0, 0);
        //tilemap.SetTile(tilePos, mainTile);

        Debug.Log(tilemap.HasTile(tilePos));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
