using UnityEngine;
using Unity.Netcode;

public class LoadTiles : MonoBehaviour
{
    public NetworkObject TilesToSpawn;

    private void OnServerInitialized()
    {
        TilesToSpawn.Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
